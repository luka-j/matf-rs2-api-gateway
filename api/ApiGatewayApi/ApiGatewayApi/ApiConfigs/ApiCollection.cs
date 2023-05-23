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

    private static readonly TimeSpan VALIDITY_MIN_OFFSET = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan PRUNE_AFTER = TimeSpan.FromMinutes(1);

    private readonly object _pruningLock = new object();

    private class ApiConfigValidityStartComparer : IComparer<ApiConfig?>
    {
        public int Compare(ApiConfig? x, ApiConfig? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return -x.ValidFrom.CompareTo(y.ValidFrom);
        }
    }

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
            _configs.Add(id, new SortedSet<ApiConfig>(new ApiConfigValidityStartComparer()));
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
        try
        {
            return eligibleConfigs.First(config => config.IsActive(now));
        }
        catch (InvalidOperationException e)
        {
            return null;
        }
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

    /// <summary>
    /// Delete versions of all configs that start after now.
    /// </summary>
    /// <param name="now">Moment in time after which to delete all configs.</param>
    /// <returns>How many config versions were deleted.</returns>
    public int RevertPendingChanges(DateTime now)
    {
        lock (_pruningLock)
        {
            var removedConfigs = 0;
            foreach (var (key, value) in _configs)
            {
                removedConfigs += value.RemoveWhere(apiConfig => apiConfig.ValidFrom >= now);
            }

            return removedConfigs;
        }
    }

    // remove all configs which are active, but not valid, i.e. for which there's a newer active config
    private void Prune(DateTime now)
    {
        lock (_pruningLock)
        {
            foreach (var (key, value) in _configs)
            {
                var configs = new SortedSet<ApiConfig>(new ApiConfigValidityStartComparer());
                ApiConfig? newest = null;
                foreach (var config in value)
                {
                    if (newest != null)
                    {
                        newest = config;
                        configs.Add(newest);
                    }

                    configs.Add(config);
                    if (config.ValidFrom + PRUNE_AFTER < now)
                    {
                        break;
                    }
                }

                _configs[key] = configs;
            }
        }
    }
    
    private static void CheckStartDateValidity(DateTime validFrom, DateTime now)
    {
        if (now + VALIDITY_MIN_OFFSET > validFrom)
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