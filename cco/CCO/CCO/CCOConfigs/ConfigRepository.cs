using CCO.Entities;

namespace CCO.CCOConfigs
{
    public class ConfigRepository
    {
        public ConfigCollection<DatabaseSource> Databases { get; set; } = new();

        public ConfigCollection<CacheSource> Caches { get; set; } = new();

        public ConfigCollection<QueueSource> Queues { get; set; } = new();

    }
}
