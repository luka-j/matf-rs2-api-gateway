using Configurator.Repositories;
using Retriever;
using Grpc.Core;

namespace Configurator.Services
{
    public class ConfigRetrieverService : ConfigRetriever.ConfigRetrieverBase
    {
        private readonly IConfigRepository _configRepository;

        public ConfigRetrieverService(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        public override async Task<Specs> GetAllFrontendConfigs(Empty request, ServerCallContext context)
        {
            return await GetConfigs("frontends");
        }

        public override async Task<Specs> GetAllBackendConfigs(Empty request, ServerCallContext context)
        {
            return await GetConfigs("backends");
        }

        public override async Task<Specs> GetAllRpConfigs(Empty request, ServerCallContext context)
        {
            return await GetConfigs("middlewares");
        }

        private async Task<Specs> GetConfigs(string category)
        {
            var frontends = await _configRepository.GetConfigsByCategory(category);
            return new Specs
            {
                Specs_ = { frontends.Select(c => new Spec
                    {
                        ApiName = c.ApiName,
                        ApiVersion = c.ApiVersion,
                        Data = c.Data
                    }) 
                }
            };
        }
    }
}