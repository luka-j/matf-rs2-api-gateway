using Grpc.Core;

namespace ApiGatewayApi.Services;

public class ConfigManagementService : ConfigManagement.ConfigManagementBase
{
    private readonly ILogger<ConfigManagementService> _logger;

    public ConfigManagementService(ILogger<ConfigManagementService> logger)
    {
        _logger = logger;
    }

    public override Task<ConfigData> GetBackendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.FromResult(new ConfigData
        {
            Data = "", 
            ValidFrom = "1970-01-01T00:00:00Z"
        });
    }
    public override Task<Empty> DeleteBackendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
    public override Task<Empty> UpdateBackendConfig(ConfigData request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }
    
    public override Task<ConfigData> GetFrontendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.FromResult(new ConfigData
        {
            Data = "", 
            ValidFrom = "1970-01-01T00:00:00Z"
        });
    }
    public override Task<Empty> DeleteFrontendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<Empty> UpdateFrontendConfig(ConfigData request, ServerCallContext context)
    {
        return Task.FromResult(new Empty());
    }

    public override Task<ConfigList> GetAllBackendConfigs(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ConfigList
        {
            Configs = { new ConfigMetadata
            {
                ApiName = "BackendService",
                ApiVersion = "v1",
                BasePath = ""
            } }
        });
    }
    public override Task<ConfigList> GetAllFrontendConfigs(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ConfigList
        {
            Configs = { new ConfigMetadata
            {
                ApiName = "FrontendService",
                ApiVersion = "v2",
                BasePath = ""
                    
            } }
        });
    }
}