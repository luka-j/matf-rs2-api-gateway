using Configurator.Entities;

namespace Configurator.Repositories
{
    public interface IConfigRepository
    {
        protected enum CATEGORIES { frontends, backends, middlewares, datasources };

        Task ModifyConfigs(IEnumerable<Config> configs);

        Task DeleteConfigs(IEnumerable<Config> configs);

        Task<IEnumerable<Config>> GetAllConfigs();

    }
}
