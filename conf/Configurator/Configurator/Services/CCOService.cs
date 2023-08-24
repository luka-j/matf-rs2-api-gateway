using Configurator.Entities;
using Configurator.GrpcServices;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using CCO;

namespace Configurator.Services
{
    public class CCOService
    {
        private readonly CCOGrpcService _ccoGrpcService;

        public CCOService(CCOGrpcService ccoGrpcService)
        {
            _ccoGrpcService = ccoGrpcService ?? throw new ArgumentNullException(nameof(ccoGrpcService));
        }

        public async Task Update(CCOSpec spec, string validFrom)
        {
            string data = GetDataString(spec);
            await _ccoGrpcService.Update(data, validFrom);
        }

        public async Task Delete(string apiName, string apiVersion)
        {
            await _ccoGrpcService.Delete(apiName, apiVersion);
        }

        public async Task<IEnumerable<CCOSpec>> GetAll()
        {
            var configs = await _ccoGrpcService.GetAllData();

            var allConfigs = configs.Select(config => ParseYamlString(config.Data));

            return allConfigs;
        }

        public async Task<CCOSpec> Get(string apiName, string apiVersion)
        {
            ConfigData data = await _ccoGrpcService.Get(apiName, apiVersion);

            return ParseYamlString(data.Data);
        }

        public static CCOSpec ParseYamlString(string yamlString)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<CCOSpec>(yamlString);
        }
        public static string GetDataString(CCOSpec spec)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return serializer.Serialize(spec);
        }
    }
}
