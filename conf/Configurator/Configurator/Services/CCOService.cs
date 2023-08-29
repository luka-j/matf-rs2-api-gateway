using Configurator.GrpcServices;
using CCO;
using System.Text.Json;
using Configurator.CCOEntities;

namespace Configurator.Services
{
    public class CCOService
    {
        private readonly CCOGrpcService _ccoGrpcService;

        public CCOService(CCOGrpcService ccoGrpcService)
        {
            _ccoGrpcService = ccoGrpcService ?? throw new ArgumentNullException(nameof(ccoGrpcService));
        }

        public async Task<IEnumerable<DatabaseSpec>> GetAllDatabases()
        {
            var configs = await _ccoGrpcService.GetAllDatabaseData();

            var allConfigs = configs.Select(config => ParseDatabaseString(config.Data));

            return allConfigs;
        }

        public async Task<DatabaseSpec> GetDatabase(string name)
        {
            ConfigData data = await _ccoGrpcService.GetDatabase(name);

            return ParseDatabaseString(data.Data);
        }
        public async Task<IEnumerable<CacheSpec>> GetAllCaches()
        {
            var configs = await _ccoGrpcService.GetAllCacheData();

            var allConfigs = configs.Select(config => ParseCacheString(config.Data));

            return allConfigs;
        }

        public async Task<CacheSpec> GetCache(string name)
        {
            ConfigData data = await _ccoGrpcService.GetCache(name);

            return ParseCacheString(data.Data);
        }
        public async Task<IEnumerable<QueueSpec>> GetAllQueues()
        {
            var configs = await _ccoGrpcService.GetAllQueueData();

            var allConfigs = configs.Select(config => ParseQueueString(config.Data));

            return allConfigs;
        }

        public async Task<QueueSpec> GetQueue(string name)
        {
            ConfigData data = await _ccoGrpcService.GetQueue(name);

            return ParseQueueString(data.Data);
        }

        public static DatabaseSpec ParseDatabaseString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<DatabaseSpec>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }
        public static CacheSpec ParseCacheString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<CacheSpec>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }

        public static QueueSpec ParseQueueString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<QueueSpec>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }

        public static string GetDatabaseString(DatabaseSpec spec)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(spec, options);
        }
        public static string GetCacheSpec(CacheSpec spec)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(spec, options);
        }
        public static string GetQueueSpec(QueueSpec spec)
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
