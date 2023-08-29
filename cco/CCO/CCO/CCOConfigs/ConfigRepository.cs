using CCO.Entities;

namespace CCO.CCOConfigs
{
    public class ConfigRepository
    {
        public ConfigCollection<DatabaseSpec> Databases { get; set; } = new();

        public ConfigCollection<CacheSpec> Caches { get; set; } = new();

        public ConfigCollection<QueueSpec> Queues { get; set; } = new();

    }
}
