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

        public async Task<IEnumerable<CCOSpec<DatabaseSource>>> GetAllDatabases()
        {
            var configs = await _ccoGrpcService.GetAllDatabaseData();

            var allConfigs = configs.Select(config => ParseDatabaseString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec<DatabaseSource>> GetDatabase(string name)
        {
            ConfigData data = await _ccoGrpcService.GetDatabase(name);

            return ParseDatabaseString(data.Data);
        }
        public async Task<IEnumerable<CCOSpec<CacheSource>>> GetAllCaches()
        {
            var configs = await _ccoGrpcService.GetAllCacheData();

            var allConfigs = configs.Select(config => ParseCacheString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec<CacheSource>> GetCache(string name)
        {
            ConfigData data = await _ccoGrpcService.GetCache(name);

            return ParseCacheString(data.Data);
        }
        public async Task<IEnumerable<CCOSpec<QueueSource>>> GetAllQueues()
        {
            var configs = await _ccoGrpcService.GetAllQueueData();

            var allConfigs = configs.Select(config => ParseQueueString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec<QueueSource>> GetQueue(string name)
        {
            ConfigData data = await _ccoGrpcService.GetQueue(name);

            return ParseQueueString(data.Data);
        }

        public static CCOSpec<DatabaseSource> ParseDatabaseString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<CCOSpec<DatabaseSource>>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }
        public static CCOSpec<CacheSource> ParseCacheString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<CCOSpec<CacheSource>>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }

        public static CCOSpec<QueueSource> ParseQueueString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<CCOSpec<QueueSource>>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }

        public static string GetDatabaseString(CCOSpec<DatabaseSource> spec)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(spec, options);
        }
        public static string GetCacheSpec(CCOSpec<CacheSource> spec)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(spec, options);
        }
        public static string GetQueueSpec(CCOSpec<QueueSource> spec)
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
