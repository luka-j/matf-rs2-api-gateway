namespace Configurator.Entities
{
    public class CCOSpec
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public IEnumerable<CCODatasource> Databases { get; set; }
        public IEnumerable<CCODatasource> Caches { get; set; }
        public IEnumerable<CCODatasource> Queues { get; set; }

        public CCOSpec(string title, string version, IEnumerable<CCODatasource> databases, IEnumerable<CCODatasource> caches, IEnumerable<CCODatasource> queues)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Databases = databases ?? throw new ArgumentNullException(nameof(databases));
            Caches = caches ?? throw new ArgumentNullException(nameof(caches));
            Queues = queues ?? throw new ArgumentNullException(nameof(queues));
        }
    }
}
