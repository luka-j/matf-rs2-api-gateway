using System.Globalization;
using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRp;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace ApiGatewayRequestProcessor.Services;

public class ConfigManagementService : ConfigManagement.ConfigManagementBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly ConfigRepository _repository;

    public ConfigManagementService(ConfigRepository repository)
    {
        _repository = repository;
    }
    
    public override Task<Empty> DeleteConfig(ConfigId request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("DeleteBackendConfig request: {Request}", request);
            var status = _repository.DeleteConfig(new ApiIdentifier(request.ApiName, request.ApiVersion));
            if (!status)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Failed to delete Config"));
            }
            return new Empty();
        });
    }
    
    public override Task<Empty> UpdateConfig(ConfigData request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("UpdateBackendConfig, ValidFrom: {ValidFrom}", request.ValidFrom);
            try {
                _repository.UpdateConfig(request.Data, DateTime.Parse(request.ValidFrom, null, DateTimeStyles.RoundtripKind));
                return new Empty();
            }
            catch (ApiConfigException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        });
    }
    
    
    public override Task<ConfigData> GetConfig(ConfigId request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetFrontendConfig request: {Request}", request);
            try
            {
                var configData = _repository.GetCurrentConfig(
                    new ApiIdentifier(request.ApiName, request.ApiVersion), DateTime.Now);
                return new ConfigData
                {
                    Data = configData?.Data,
                    ValidFrom = configData?.ValidFrom.ToString("O")
                };
            }
            catch (Exception ex) when(ex is KeyNotFoundException or InvalidOperationException) 
            {
                throw new RpcException(new Status(StatusCode.NotFound, "No active Config with such id exists"));
            }
        });
    }
    
    public override Task<RevertChangesResponse> RevertPendingUpdates(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            DateTime now = DateTime.Now;
            _logger.Information("RevertPendingUpdates, time: {Now}", now);
            int revertedChanges = _repository.RevertPendingChanges(now);
            return new RevertChangesResponse
            {
                RevertedChanges = revertedChanges, 
            };
        });
    }


    public override Task<ConfigList> GetAllConfigs(Empty request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("GetAllBackendConfigs request: {Request}", request);
            var configMetadata = _repository.GetAllConfigs(DateTime.Now)
                .Select(metadata => new ConfigMetadata
                {
                    ApiName = metadata.Name,
                    ApiVersion = metadata.Version
                });
            var ret = new ConfigList();
            ret.Configs.AddRange(configMetadata);
            return ret;
        });
    }
}