namespace ApiGatewayApi.ApiConfigs;

/// <summary>
/// Holds two API collections, for frontends and for backends. Frontends are typically
/// used by HTTP controller, and backends by gRPC service for invoking HTTP calls.
/// </summary>
public class ApiRepository
{
    public ApiCollection Frontends { get; } = new();
    public ApiCollection Backends { get; } = new();

}