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
        public async Task UpdateDatabase(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };

            foreach (var client in _configManagementClients)
            {
                await client.UpdateDatabaseConfigAsync(updateRequest);
            }
        }

        public async Task DeleteDatabase(string name)
        {
            ConfigId deleteRequest = new()
            {
                Name = name
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteDatabaseConfigAsync(deleteRequest);
            }
        }

        public async Task<ConfigList> GetAllDatabases()
        {
            var client = _configManagementClients.First();
            return await client.GetAllDatabaseConfigsAsync(new Empty());
        }
        
        public async Task<IEnumerable<ConfigData>> GetAllDatabaseData()
        {
            var client = _configManagementClients.First();
            var configList = await client.GetAllDatabaseConfigsAsync(new Empty());

            IEnumerable<ConfigData> configs = new List<ConfigData>();

            foreach(ConfigId configId in configList.Configs)
            {
                configs = configs.Append(await client.GetDatabaseConfigAsync(configId));
            }
            return configs;
        }
        public async Task<ConfigData> GetDatabase(string name)
        {
            ConfigId getRequest = new()
            {
                Name = name
            };
            var client = _configManagementClients.First();
            return await client.GetDatabaseConfigAsync(getRequest);
        }
        public async Task UpdateCache(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };

            foreach (var client in _configManagementClients)
            {
                await client.UpdateCacheConfigAsync(updateRequest);
            }
        }

        public async Task DeleteCache(string name)
        {
            ConfigId deleteRequest = new()
            {
                Name = name
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteCacheConfigAsync(deleteRequest);
            }
        }

        public async Task<ConfigList> GetAllCaches()
        {
            var client = _configManagementClients.First();
            return await client.GetAllCacheConfigsAsync(new Empty());
        }

        public async Task<IEnumerable<ConfigData>> GetAllCacheData()
        {
            var client = _configManagementClients.First();
            var configList = await client.GetAllCacheConfigsAsync(new Empty());

            IEnumerable<ConfigData> configs = new List<ConfigData>();

            foreach (ConfigId configId in configList.Configs)
            {
                configs = configs.Append(await client.GetCacheConfigAsync(configId));
            }
            return configs;
        }
        public async Task<ConfigData> GetCache(string name)
        {
            ConfigId getRequest = new()
            {
                Name = name
            };
            var client = _configManagementClients.First();
            return await client.GetCacheConfigAsync(getRequest);
        }
        public async Task UpdateQueue(string data, string validFrom)
        {
            ConfigData updateRequest = new()
            {
                Data = data,
                ValidFrom = validFrom
            };

            foreach (var client in _configManagementClients)
            {
                await client.UpdateQueueConfigAsync(updateRequest);
            }
        }

        public async Task DeleteQueue(string name)
        {
            ConfigId deleteRequest = new()
            {
                Name = name
            };
            foreach (var client in _configManagementClients)
            {
                await client.DeleteQueueConfigAsync(deleteRequest);
            }
        }

        public async Task<ConfigList> GetAllQueues()
        {
            var client = _configManagementClients.First();
            return await client.GetAllQueueConfigsAsync(new Empty());
        }

        public async Task<IEnumerable<ConfigData>> GetAllQueueData()
        {
            var client = _configManagementClients.First();
            var configList = await client.GetAllQueueConfigsAsync(new Empty());

            IEnumerable<ConfigData> configs = new List<ConfigData>();

            foreach (ConfigId configId in configList.Configs)
            {
                configs = configs.Append(await client.GetQueueConfigAsync(configId));
            }
            return configs;
        }
        public async Task<ConfigData> GetQueue(string name)
        {
            ConfigId getRequest = new()
            {
                Name = name
            };
            var client = _configManagementClients.First();
            return await client.GetQueueConfigAsync(getRequest);
        }
        public async Task RevertPendingChanges()
        {
            foreach (var client in _configManagementClients)
            {
                await client.RevertPendingUpdatesAsync(new Empty());
            }
        }
    }
}
