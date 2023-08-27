namespace CCO.CCOConfigs
{
    public class ConfigRepository
    {
        public ConfigCollection Databases { get; set; } = new();

        public ConfigCollection Caches { get; set; } = new();

        public ConfigCollection Queues { get; set; } = new();

    }
}
