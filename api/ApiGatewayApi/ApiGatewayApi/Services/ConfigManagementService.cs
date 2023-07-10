using System.Globalization;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Services;

public class ConfigManagementService : ConfigManagement.ConfigManagementBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly ApiRepository _repository;

    public ConfigManagementService(ApiRepository repository)
    {
        _repository = repository;
    }

    public override Task<ConfigData> GetBackendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetBackendConfig request: {Request}", request);
            try
            {
                var configData = _repository.Backends.GetCurrentConfig(
                    new ApiIdentifier(request.ApiName, request.ApiVersion), DateTime.Now);
                return new ConfigData
                {
                    Data = configData?.GetSpecString(),
                    ValidFrom = configData?.ValidFrom.ToString(CultureInfo.InvariantCulture)
                };
            }
            catch (Exception ex) when(ex is KeyNotFoundException or InvalidOperationException) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No active Config with such id exists"));
            }
        });
    }
    
    public override Task<Empty> DeleteBackendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("DeleteBackendConfig request: {Request}", request);
            var status = _repository.Backends.DeleteConfig(new ApiIdentifier(request.ApiName, request.ApiVersion));
            if (!status)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
            }
            return new Empty();
        });
    }
    
    public override Task<Empty> UpdateBackendConfig(ConfigData request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("UpdateBackendConfig, ValidFrom: {ValidFrom}", request.ValidFrom);
            try {
                _repository.Backends.AddConfig(new ApiSpec(request.Data, DateTime.Parse(request.ValidFrom)));
                return new Empty();
            }
            catch (ApiConfigException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        });
    }
    
    
    public override Task<ConfigData> GetFrontendConfig(ConfigId request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetFrontendConfig request: {Request}", request);
            try
            {
                var configData = _repository.Frontends.GetCurrentConfig(
                    new ApiIdentifier(request.ApiName, request.ApiVersion), DateTime.Now);
                return new ConfigData
                {
                    Data = configData?.GetSpecString(),
                    ValidFrom = configData?.ValidFrom.ToString(CultureInfo.InvariantCulture)
                };
            }
            catch (Exception ex) when(ex is KeyNotFoundException or InvalidOperationException) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No active Config with such id exists"));
            }
        });
    }
    
    public override Task<Empty> DeleteFrontendConfig(ConfigId request, ServerCallContext context)
    {        return Task.Run(() =>
        {
            _logger.Information("DeleteFrontendConfig request: {Request}", request);
            var status = _repository.Frontends.DeleteConfig(new ApiIdentifier(request.ApiName, request.ApiVersion));
            if (!status)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
            }
            return new Empty();
        });
    }

    public override Task<Empty> UpdateFrontendConfig(ConfigData request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("UpdateFrontendConfig ValidFrom: {ValidFrom}", request.ValidFrom);
            try
            {
                _repository.Frontends.AddConfig(new ApiSpec(request.Data, DateTime.Parse(request.ValidFrom)));
                return new Empty();
            }
            catch (ApiConfigException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        });
    }

    public override Task<RevertChangesResponse> RevertPendingUpdates(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            DateTime now = DateTime.Now;
            _logger.Information("RevertPendingUpdates, time: {Now}", now);
            int revertedFrontends = _repository.Frontends.RevertPendingChanges(now);
            int revertedBackends = _repository.Backends.RevertPendingChanges(now);
            return new RevertChangesResponse
            {
                RevertedFrontends = revertedFrontends, 
                RevertedBackends = revertedBackends
            };
        });
    }


    public override Task<ConfigList> GetAllBackendConfigs(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetAllBackendConfigs request: {Request}", request);
            var configMetadata = _repository.Backends.GetAllConfigsMetadata(DateTime.Now)
                .Select(metadata => new ConfigMetadata
                {
                    ApiName = metadata.Name,
                    ApiVersion = metadata.Version,
                    BasePath = metadata.BasePath
                });
            var ret = new ConfigList();
            ret.Configs.AddRange(configMetadata);
            return ret;
        });
    }
    
    public override Task<ConfigList> GetAllFrontendConfigs(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetAllBackendConfigs request: {Request}", request);
            var configMetadata = _repository.Frontends.GetAllConfigsMetadata(DateTime.Now)
                .Select(metadata => new ConfigMetadata
                {
                    ApiName = metadata.Name,
                    ApiVersion = metadata.Version,
                    BasePath = metadata.BasePath
                });
            var ret = new ConfigList();
            ret.Configs.AddRange(configMetadata);
            return ret;
        });
    }
}