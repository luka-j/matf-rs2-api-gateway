using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.ApiConfigs;

/// <summary>
/// Represents a collection of API configs. Provides methods for managing those configs.
/// </summary>
public class ApiCollection
{
    private readonly Dictionary<ApiIdentifier, SortedSet<ApiConfig>> _configs = new();
    private readonly ILogger _logger = Serilog.Log.Logger;

    /// <summary>
    /// Add a new config to the collection. Config's validity start must be in the future and ApiSpec's
    /// spec string must be a valid OAS3 spec.
    /// </summary>
    /// <param name="spec">ApiSpec containing spec string and validity start time.</param>
    /// <exception cref="ApiConfigException">If there's an issue with the config, e.g. validity
    /// start date is too early or config is in bad format.</exception>
    public void AddConfig(ApiSpec spec)
    {
        var now = DateTime.Now;
        CheckStartDateValidity(spec.ValidFrom, now);
        var parsedOas = ParseSpec(spec.Spec);

        var parsedConfig = new ApiConfig(parsedOas, spec.ValidFrom);
        var id = parsedConfig.Id;
        if (!_configs.ContainsKey(id))
        {
            _configs.Add(id, new SortedSet<ApiConfig>(Comparer<ApiConfig>.Create((c1, c2) => c2.ValidFrom.CompareTo(c1.ValidFrom))));
        }

        _configs[id].Add(parsedConfig);
        Prune(now);
    }

    /// <summary>
    /// Check whether this collection has an ApiConfig corresponding to a given id.
    /// </summary>
    /// <param name="id">ApiIdentifier of the desired config.</param>
    /// <returns>true if this collection has the config, false otherwise.</returns>
    public bool HasConfig(ApiIdentifier id)
    {
        return _configs.ContainsKey(id);
    }

    /// <summary>
    /// Get all current configs metadata.
    /// </summary>
    /// <param name="now">Current time.</param>
    /// <returns>IEnumerable of ApiMetadata objects, each representing a single valid config.</returns>
    public IEnumerable<ApiMetadata> GetAllConfigsMetadata(DateTime now)
    {
        return _configs.Select(kv => GetCurrentConfig(kv.Key, now))
            .Where(kv => kv != null)
            .Select(config => config!.GetMetadata());
    }

    /// <summary>
    /// Get current config for a given id. Current (or valid) config is the config with
    /// the latest validity start time which is before or equal current time.
    /// </summary>
    /// <param name="id">id of the desired config</param>
    /// <param name="now">Current time</param>
    /// <returns>ApiConfig if it exists in this collection, false otherwise.</returns>
    public ApiConfig? GetCurrentConfig(ApiIdentifier id, DateTime now)
    {
        if (!_configs.ContainsKey(id)) return null;
        var eligibleConfigs = _configs[id];
        return eligibleConfigs.First(config => config.IsActive(now));
    }

    /// <summary>
    /// Delete all versions (active, not yet active and past) of a config identified by the given id.
    /// </summary>
    /// <param name="id">id of the config to delete</param>
    /// <returns>Whether the deletion succeeded (it can be false if e.g. there's no config with such id).</returns>
    public bool DeleteConfig(ApiIdentifier id)
    {
        return _configs.Remove(id);
    }

    // remove all configs which are active, but not valid, i.e. for which there's a newer active config
    private void Prune(DateTime now)
    {
        foreach (var (key, value) in _configs)
        {
            var configs = new SortedSet<ApiConfig>();
            foreach (var config in value)
            {
                if (config.ValidFrom.AddSeconds(5) >= now)
                {
                    configs.Add(config);
                    continue;
                }

                configs.Add(config);
                break;
            }

            _configs[key] = configs;
        }
    }
    
    private static void CheckStartDateValidity(DateTime validFrom, DateTime now)
    {
        if (now.AddSeconds(1) < validFrom)
        {
            throw new ApiConfigException("API config validity is set to a too early time!");
        }
    }

    private OpenApiDocument ParseSpec(string spec)
    {
        var reader = new OpenApiStringReader();
        var doc = reader.Read(spec, out var diagnostic);
        if (diagnostic.Errors.Count > 0)
        {
            _logger.Error("Errors occurred while parsing spec: {errors}. Warnings: {warnings}", 
                diagnostic.Errors, diagnostic.Warnings);
            throw new ApiConfigException("Failed to parse API spec. Check log for details");
        }

        if (diagnostic.Warnings.Count > 0)
        {
            _logger.Warning("Warnings encountered while parsing spec {metadata}: {warnings}", 
                doc.Info, diagnostic.Warnings);
        }

        if (doc == null)
        {
            throw new ApiConfigException("Parsed spec to null. This should not happen!");
        }

        return doc;
    }
}