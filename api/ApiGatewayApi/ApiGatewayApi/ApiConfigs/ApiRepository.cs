namespace ApiGatewayApi.ApiConfigs;

public class ApiRepository
{
    public ApiCollection Frontends { get; } = new();
    public ApiCollection Backends { get; }= new();

}