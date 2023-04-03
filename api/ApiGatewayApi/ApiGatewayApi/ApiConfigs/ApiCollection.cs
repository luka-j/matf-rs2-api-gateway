using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.ApiConfigs;

public class ApiCollection
{
    private readonly Dictionary<ApiIdentifier, SortedSet<ApiConfig>> _configs = new();
    private readonly ILogger _logger = Serilog.Log.Logger;

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

    public bool HasConfig(ApiIdentifier id)
    {
        return _configs.ContainsKey(id);
    }

    public IEnumerable<ApiMetadata> GetAllConfigsMetadata(DateTime now)
    {
        return _configs.Select(kv => GetCurrentConfig(kv.Key, now))
            .Where(kv => kv != null)
            .Select(config => config!.GetMetadata());
    }

    public ApiConfig? GetCurrentConfig(ApiIdentifier id, DateTime now)
    {
        var eligibleConfigs = _configs[id];
        return eligibleConfigs.First(config => config.IsActive(now));
    }

    public bool DeleteConfig(ApiIdentifier id)
    {
        return _configs.Remove(id);
    }

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