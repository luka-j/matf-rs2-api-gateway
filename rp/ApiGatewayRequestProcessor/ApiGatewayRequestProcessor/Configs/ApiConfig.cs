using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Steps;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiConfig
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    public string Name { get; set; }
    public string Version { get; set; }

    public List<string> Include
    {
        set
        {
            _includes = value.Select(i =>
            {
                var parts = i.Split("/");
                if (parts.Length != 2)
                {
                    throw new ApiConfigException("Invalid include path: " + i + ". Includes should be in format " +
                                                 "Name/Version");
                }

                return new ApiIdentifier(parts[0].Trim(), parts[1].Trim());
            }).ToList();
        }
    }

    public Dictionary<string, ApiEndpoint> Endpoints { get; set; }
    
    public Dictionary<string, List<Step>> Steps { get; set; }

    private List<ApiIdentifier>? _includes;

    private readonly HashSet<ApiSpec> _processedIncludes = new();
    
    public bool HasOperation(string path, string method)
    {
        return Endpoints.TryGetValue(path, out var endpoint) && endpoint.Operations.ContainsKey(method);
    }
    
    public Task<ExecutionResponse> Execute(string path, string method, 
        ExecutionRequest request, ApiGateway apiGateway)
    {
        return Endpoints[path].Operations[method].Execute(request, apiGateway, Steps);
    }

    public void ResolveIncludes(ConfigRepository repo, DateTime now)
    {
        if (_includes == null) return;
        
        foreach (var id in _includes)
        {
            if (!repo.HasConfig(id))
            {
                _logger.Warning("Cannot resolve include {ApiIdentifier} in Config {Name}/{Version}: " +
                                "included config is not present in repository", id, Name, Version);
                continue;
            }

            var spec = repo.GetCurrentConfig(id, now);
            if (spec == null)
            {
                _logger.Warning("Cannot resolve include {ApiIdentifier} in Config {Name}/{Version}: " +
                                "included config is not present in repository at {Now}", id, Name, Version, now);
                continue;
            }
            
            if (_processedIncludes.Contains(spec)) continue;
            foreach (var (key, steps) in spec.Config.Steps)
            {
                Steps.TryAdd(key, steps);
            }

            _processedIncludes.Add(spec);
        }
    }
}