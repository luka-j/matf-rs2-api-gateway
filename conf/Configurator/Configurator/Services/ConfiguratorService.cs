using Configurator.Entities;
using Configurator.GrpcServices;
using Configurator.Repositories;

namespace Configurator.Services
{
    public class ConfiguratorService
    {
        private const int VALIDFROM_SECONDS = 10;
        private const int TIMEOUT_SECONDS = 5;

        private readonly IConfigRepository _configRepository;

        private readonly APIGrpcService _apiService;
        private readonly RPGrpcService _rpService;
        private readonly CCOGrpcService _ccoService;

        public ConfiguratorService(IConfigRepository configRepository, APIGrpcService apiService, RPGrpcService rpService, CCOGrpcService ccoService)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _rpService = rpService ?? throw new ArgumentNullException(nameof(rpService));
            _ccoService = ccoService ?? throw new ArgumentNullException(nameof(ccoService));
        }

        public async Task<bool> Update(IEnumerable<Config> configs)
        {
            DateTime validFrom = DateTime.Now.AddSeconds(VALIDFROM_SECONDS);
            DateTime timeout = DateTime.Now.AddSeconds(TIMEOUT_SECONDS);

            List<Task<bool>> completes = new();

            foreach (var config in configs)
            {
                Task? task = null;
                switch (config.Category)
                {
                    case "frontends":
                        task = _apiService.UpdateFrontend(config.Data, validFrom.ToString());
                        break;
                    case "backends":
                        task = _apiService.UpdateBackend(config.Data, validFrom.ToString());
                        break;
                    case "middlewares":
                        task = _rpService.Update(config.Data, validFrom.ToString());
                        break;
                    case "datasources":
                        task = _ccoService.Update(config.Data, validFrom.ToString());
                        break;
                    default:
                        break;
                }
                if (task != null)
                {
                    completes.Add(WaitWithTimeout(task, timeout));
                }
            }

            foreach (var complete in completes)
                if (!await complete)
                {
                    await _apiService.RevertPendingChanges();
                    await _rpService.RevertPendingChanges();
                    await _ccoService.RevertPendingChanges();
                    return false;
                };

            return true;
        }
        public async Task<IEnumerable<Config>> GetAllConfigs()
        {
            return await _configRepository.GetAllConfigs();
        }
        public async Task<IEnumerable<Config>> GetConfigsByCategory(string category)
        {
            return await _configRepository.GetConfigsByCategory(category);
        }
        public async Task<bool> UpdateConfigs()
        {
            var configs = await _configRepository.GetAllConfigs();

            return await Update(configs);
        }

        public async Task<bool> ModifyAndUpdate(IEnumerable<Config> configs)
        {
            var newConfigs = await _configRepository.ModifyConfigs(configs);

            return await Update(newConfigs);
        }

        public async Task<bool> DeleteConfigs(IEnumerable<ConfigId> configs)
        {
            try
            {
                var deletedConfigs = await _configRepository.DeleteConfigs(configs);

                foreach (var config in configs)
                {
                    switch (config.Category)
                    {
                        case "frontends":
                            await _apiService.DeleteFrontend(config.ApiName, config.ApiVersion);
                            break;
                        case "backends":
                            await _apiService.DeleteBackend(config.ApiName, config.ApiVersion);
                            break;
                        case "middlewares":
                            await _rpService.Delete(config.ApiName, config.ApiVersion);
                            break;
                        case "datasources":
                            await _ccoService.Delete(config.ApiName, config.ApiVersion);
                            break;
                        default:
                            break;
                    }
                }

            } catch { return false; }

            return true;
        }
            

        private static async Task<bool> WaitWithTimeout(Task task, DateTime timeout)
        {
            int tout = (int)timeout.Subtract(DateTime.Now).TotalMilliseconds;
            if (await Task.WhenAny(task, Task.Delay(tout)) == task)
            {
                return true;
            }
            return false;
        }


    }
}
