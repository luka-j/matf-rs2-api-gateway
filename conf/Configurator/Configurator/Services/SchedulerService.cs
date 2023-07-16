namespace Configurator.Services
{
    public class SchedulerService
    {
        private const int PERIOD_SECONDS = 60;
        private readonly IServiceScopeFactory _scopeFactory;

        public SchedulerService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _ = UpdatePeriodically();
        }

        private async Task Update()
        {
            using var scope = _scopeFactory.CreateScope();
            var configurator = scope.ServiceProvider.GetService<ConfiguratorService>();
            if (configurator != null) await configurator.UpdateConfigs();
        }

        private async Task UpdatePeriodically()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(PERIOD_SECONDS));
            while (await timer.WaitForNextTickAsync()) { await Update(); }
        }
    }
}
