using Configurator.Entities;
using Configurator.GrpcServices;
using CCO;
using System.Text.Json;

namespace Configurator.Services
{
    public class CCOService
    {
        private readonly CCOGrpcService _ccoGrpcService;

        public CCOService(CCOGrpcService ccoGrpcService)
        {
            _ccoGrpcService = ccoGrpcService ?? throw new ArgumentNullException(nameof(ccoGrpcService));
        }

        public async Task<IEnumerable<CCOSpec>> GetAllDatabases()
        {
            var configs = await _ccoGrpcService.GetAllDatabaseData();

            var allConfigs = configs.Select(config => ParseJsonString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec> GetDatabase(string name)
        {
            ConfigData data = await _ccoGrpcService.GetDatabase(name);

            return ParseJsonString(data.Data);
        }
        public async Task<IEnumerable<CCOSpec>> GetAllCaches()
        {
            var configs = await _ccoGrpcService.GetAllCacheData();

            var allConfigs = configs.Select(config => ParseJsonString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec> GetCache(string name)
        {
            ConfigData data = await _ccoGrpcService.GetCache(name);

            return ParseJsonString(data.Data);
        }
        public async Task<IEnumerable<CCOSpec>> GetAllQueues()
        {
            var configs = await _ccoGrpcService.GetAllQueueData();

            var allConfigs = configs.Select(config => ParseJsonString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec> GetQueue(string name)
        {
            ConfigData data = await _ccoGrpcService.GetQueue(name);

            return ParseJsonString(data.Data);
        }

        public static CCOSpec ParseJsonString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<CCOSpec>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }
        public static string GetDataString(CCOSpec spec)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(spec, options);
        }

        
    }
}
