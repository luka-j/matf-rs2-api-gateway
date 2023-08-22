using CCO;

namespace Configurator.GrpcServices
{
    public class CCOGrpcService
    {
        private readonly IClientGenerator _clientGenerator;
        private readonly IEnumerable<ConfigManagement.ConfigManagementClient> _configManagementClients;

        public CCOGrpcService(IClientGenerator clientGenerator)
        {
            _clientGenerator = clientGenerator ?? throw new ArgumentNullException(nameof(clientGenerator));
            _configManagementClients = _clientGenerator.GetCCOClients();
        }
        public async Task Update(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };

            foreach (var client in _configManagementClients)
            {
                await client.UpdateConfigAsync(updateRequest);
            }
        }

        public async Task Delete(string apiName, string apiVersion)
        {
            ConfigId deleteRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteConfigAsync(deleteRequest);
            }
        }

        public async Task RevertPendingChanges()
        {
            foreach (var client in _configManagementClients)
            {
                await client.RevertPendingUpdatesAsync(new Empty());
            }
        }

        public async Task<ConfigList> GetAll()
        {
            var client = _configManagementClients.First();
            return await client.GetAllConfigsAsync(new Empty());
        }

        public async Task<ConfigData> Get(string apiName, string apiVersion)
        {
            ConfigId getRequest = new()
            {
                ApiName = apiName,
                ApiVersion = apiVersion
            };
            var client = _configManagementClients.First();
            return await client.GetConfigAsync(getRequest);
        }
    }
}
