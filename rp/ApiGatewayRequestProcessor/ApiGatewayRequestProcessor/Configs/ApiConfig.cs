namespace ApiGatewayRequestProcessor.Configs;

public class ApiConfig
{
    public string Name { get; set; }
    public string Version { get; set; }
    
    public Dictionary<string, ApiEndpoint> Endpoints { get; set; }
}