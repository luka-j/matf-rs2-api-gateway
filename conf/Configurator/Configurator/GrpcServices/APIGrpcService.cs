using ApiGatewayApi;

namespace Configurator.GrpcServices
{
    public class APIGrpcService
    {
        private readonly ConfigManagement.ConfigManagementClient _configManagementClient;

        public APIGrpcService(ConfigManagement.ConfigManagementClient configManagementClient)
        {
            _configManagementClient = configManagementClient ?? throw new ArgumentNullException(nameof(configManagementClient));
        }

        public async Task<Empty> UpdateFrontend(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };
            return await _configManagementClient.UpdateFrontendConfigAsync(updateRequest);
        }

        public async Task<Empty> DeleteFrontend(string apiName, string apiVersion)
        {
            ConfigId deleteRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            return await _configManagementClient.DeleteFrontendConfigAsync(deleteRequest);
        }

        public async Task<Empty> UpdateBackend(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };
            return await _configManagementClient.UpdateBackendConfigAsync(updateRequest);
        }

        public async Task<Empty> DeleteBackend(string apiName, string apiVersion)
        {
            ConfigId deleteRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            return await _configManagementClient.DeleteBackendConfigAsync(deleteRequest);
        }

        public async Task<RevertChangesResponse> RevertPendingChanges()
        {
            return await _configManagementClient.RevertPendingUpdatesAsync(new Empty());
        }

        public async Task<ConfigList> GetAllFrontend()
        {
            return await _configManagementClient.GetAllFrontendConfigsAsync(new Empty());
        }

        public async Task<ConfigData> GetFrontend(string apiName, string apiVersion)
        {
            ConfigId getRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            return await _configManagementClient.GetFrontendConfigAsync(getRequest);
        } 

        public async Task<ConfigList> GetAllBackend()
        {
            return await _configManagementClient.GetAllBackendConfigsAsync(new Empty());
        }

        public async Task<ConfigData> GetBackend(string apiName, string apiVersion)
        {
            ConfigId getRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            return await _configManagementClient.GetBackendConfigAsync(getRequest);
        }
    }
}
