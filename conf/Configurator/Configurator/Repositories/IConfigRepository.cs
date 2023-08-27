using Configurator.Entities;

namespace Configurator.Repositories
{
    public interface IConfigRepository
    {
        protected enum CATEGORIES { frontends, backends, middlewares, datasources };

        protected enum DATASOURCES { databases, caches, queues };

        Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs);

        Task<IEnumerable<ConfigId>> DeleteConfigs(IEnumerable<ConfigId> configs);

        Task<IEnumerable<Config>> GetAllConfigs();

        Task<IEnumerable<Config>> GetConfigsByCategory(string category);

    }
}
