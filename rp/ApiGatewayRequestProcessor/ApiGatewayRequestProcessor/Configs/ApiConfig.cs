namespace ApiGatewayRequestProcessor.Configs;

public class ApiConfig
{
    public string Name { get; set; }
    public string Version { get; set; }
    
    public Dictionary<string, ApiEndpoint> Endpoints { get; set; }

    public ApiOperation? ResolveOperation(string path, string method)
    {
        if (!Endpoints.TryGetValue(path, out var endpoint))
        {
            return null;
        }

        if (!endpoint.Operations.TryGetValue(method, out var operation))
        {
            return null;
        }

        return operation;
    }
}