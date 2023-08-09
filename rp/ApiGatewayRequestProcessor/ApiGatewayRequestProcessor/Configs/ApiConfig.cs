using ApiGatewayApi;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Steps;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiConfig
{
    public string Name { get; set; }
    public string Version { get; set; }
    
    public Dictionary<string, ApiEndpoint> Endpoints { get; set; }
    
    public Dictionary<string, List<Step>> Steps { get; set; }

    public bool HasOperation(string path, string method)
    {
        return Endpoints.TryGetValue(path, out var endpoint) && endpoint.Operations.ContainsKey(method);
    }
    
    public Task<ExecutionResponse> Execute(string path, string method, 
        ExecutionRequest request, ApiGateway apiGateway)
    {
        return Endpoints[path].Operations[method].Execute(request, apiGateway, Steps);
    }
}