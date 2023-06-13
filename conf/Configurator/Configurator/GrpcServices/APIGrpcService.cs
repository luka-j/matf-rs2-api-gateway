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
            var updateRequest = new ConfigData();
            updateRequest.Data = data;
            updateRequest.ValidFrom = validFrom;
            return await _configManagementClient.UpdateFrontendConfigAsync(updateRequest);
        }

        public async Task<Empty> DeleteFrontend(string apiName, string apiVersion)
        {
            var deleteRequest = new ConfigId();
            deleteRequest.ApiName = apiName;    
            deleteRequest.ApiVersion = apiVersion;
            return await _configManagementClient.DeleteFrontendConfigAsync(deleteRequest);
        }

        public async Task<Empty> UpdateBackend(string data, string validFrom)
        {
            var updateRequest = new ConfigData();
            updateRequest.Data = data;
            updateRequest.ValidFrom = validFrom;
            return await _configManagementClient.UpdateBackendConfigAsync(updateRequest);
        }

        public async Task<Empty> DeleteBackend(string apiName, string apiVersion)
        {
            var deleteRequest = new ConfigId();
            deleteRequest.ApiName = apiName;
            deleteRequest.ApiVersion = apiVersion;
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
            var getRequest = new ConfigId();
            getRequest.ApiName = apiName;
            getRequest.ApiVersion = apiVersion;
            return await _configManagementClient.GetFrontendConfigAsync(getRequest);
        } 

        public async Task<ConfigList> GetAllBackend()
        {
            return await _configManagementClient.GetAllBackendConfigsAsync(new Empty());
        }

        public async Task<ConfigData> GetBackend(string apiName, string apiVersion)
        {
            var getRequest = new ConfigId();
            getRequest.ApiName = apiName;
            getRequest.ApiVersion = apiVersion;
            return await _configManagementClient.GetBackendConfigAsync(getRequest);
        }
    }
}
