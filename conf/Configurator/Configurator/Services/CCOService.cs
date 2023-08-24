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

        public async Task<IEnumerable<CCOSpec>> GetAll()
        {
            var configs = await _ccoGrpcService.GetAllData();

            var allConfigs = configs.Select(config => ParseJsonString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec> Get(string apiName, string apiVersion)
        {
            ConfigData data = await _ccoGrpcService.Get(apiName, apiVersion);

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
