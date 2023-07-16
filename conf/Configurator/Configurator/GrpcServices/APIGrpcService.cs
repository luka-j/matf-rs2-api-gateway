using ApiGatewayApi;
using Grpc.Net.ClientFactory;
using Microsoft.IdentityModel.Tokens;

namespace Configurator.GrpcServices
{
    public class APIGrpcService
    {
        private readonly IClientNameService _clientNameService;
        private readonly IEnumerable<ConfigManagement.ConfigManagementClient> _configManagementClients;

        public APIGrpcService(IClientNameService clientNameService, GrpcClientFactory clientFactory)
        {
            _clientNameService = clientNameService ?? throw new ArgumentNullException(nameof(clientNameService));
            var clients = new List<ConfigManagement.ConfigManagementClient>();

            foreach (var clientName in _clientNameService.GetAPIClientNames())
            {
                try
                {
                    clients.Add(clientFactory.CreateClient<ConfigManagement.ConfigManagementClient>(clientName));
                }
                catch {}
            }
            _configManagementClients = clients;
            if (_configManagementClients.IsNullOrEmpty()) throw new ArgumentNullException(nameof(clientFactory));

        }
        public async Task UpdateFrontend(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };

            foreach (var client in _configManagementClients)
            {
                await client.UpdateFrontendConfigAsync(updateRequest);
            }
        }

        public async Task DeleteFrontend(string apiName, string apiVersion)
        {
            ConfigId deleteRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteFrontendConfigAsync(deleteRequest);
            }
        }

        public async Task UpdateBackend(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };
            foreach (var client in _configManagementClients)
            {
                await client.UpdateBackendConfigAsync(updateRequest);
            }
        }

        public async Task DeleteBackend(string apiName, string apiVersion)
        {
            ConfigId deleteRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteBackendConfigAsync(deleteRequest);
            }
        }

        public async Task RevertPendingChanges()
        {
            // TODO: check if responses are the same
            foreach (var client in _configManagementClients)
            {
                await client.RevertPendingUpdatesAsync(new Empty());
            }
        }

        public async Task<ConfigList> GetAllFrontend()
        {
            // take from first?
            // take from all and compare?
            var client = _configManagementClients.First();
            return await client.GetAllFrontendConfigsAsync(new Empty());
        }

        public async Task<ConfigData> GetFrontend(string apiName, string apiVersion)
        {
            ConfigId getRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            var client = _configManagementClients.First();
            return await client.GetFrontendConfigAsync(getRequest);
        } 

        public async Task<ConfigList> GetAllBackend()
        {
            var client = _configManagementClients.First();
            return await client.GetAllBackendConfigsAsync(new Empty());
        }

        public async Task<ConfigData> GetBackend(string apiName, string apiVersion)
        {
            ConfigId getRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion

            };
            var client = _configManagementClients.First();
            return await client.GetBackendConfigAsync(getRequest);
        }
    }
}
