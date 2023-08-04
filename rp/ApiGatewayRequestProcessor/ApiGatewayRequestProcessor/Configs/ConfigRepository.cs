using ApiGatewayRequestProcessor.ApiConfigs;
using ApiGatewayRequestProcessor.Exceptions;

namespace ApiGatewayRequestProcessor.Configs;

public class ConfigRepository
{
    private readonly Dictionary<ApiIdentifier, SortedSet<ApiSpec>> _configs = new();

    private static readonly TimeSpan VALIDITY_MIN_OFFSET = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan PRUNE_AFTER = TimeSpan.FromMinutes(1);

    private readonly object _pruningLock = new();

    private class ApiConfigValidityStartComparer : IComparer<ApiSpec?>
    {
        public int Compare(ApiSpec? x, ApiSpec? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return -x.ValidFrom.CompareTo(y.ValidFrom);
        }
    }
    
    public void UpdateConfig(string data, DateTime validFrom)
    {        
        var now = DateTime.Now;
        CheckStartDateValidity(validFrom, now);

        var parsedConfig = new ApiSpec(data, validFrom);
        var id = parsedConfig.Id;
        if (!_configs.ContainsKey(id))
        {
            _configs.Add(id, new SortedSet<ApiSpec>(new ApiConfigValidityStartComparer()));
        }

        _configs[id].Add(parsedConfig);
        Prune(now);
    }

    public bool HasConfig(ApiIdentifier id)
    {
        return _configs.ContainsKey(id);
    }
    
    public bool DeleteConfig(ApiIdentifier id)
    {
        return _configs.Remove(id);
    }

    public ApiSpec? GetCurrentConfig(ApiIdentifier id, DateTime now)
    {        
        if (!_configs.ContainsKey(id)) return null;
        var eligibleConfigs = _configs[id];
        try
        {
            return eligibleConfigs.First(config => config.IsActive(now));
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public IEnumerable<ApiMetadata> GetAllConfigs(DateTime now)
    {        
        return _configs.Select(kv => GetCurrentConfig(kv.Key, now))
            .Where(kv => kv != null)
            .Select(config => config!.GetMetadata());
    }
    
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
    
    private void Prune(DateTime now)
    {
        lock (_pruningLock)
        {
            foreach (var (key, value) in _configs)
            {
                var configs = new SortedSet<ApiSpec>(new ApiConfigValidityStartComparer());
                ApiSpec? newest = null;
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
}