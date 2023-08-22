namespace CCO.Entities
{
    public class Spec
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public IEnumerable<Datasource> Databases { get; set; }
        public IEnumerable<Datasource> Caches { get; set; }
        public IEnumerable<Datasource> Queues { get; set; }

        public Spec(string title, string version, IEnumerable<Datasource> databases, IEnumerable<Datasource> caches, IEnumerable<Datasource> queues)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Databases = databases ?? throw new ArgumentNullException(nameof(databases));
            Caches = caches ?? throw new ArgumentNullException(nameof(caches));
            Queues = queues ?? throw new ArgumentNullException(nameof(queues));
        }
    }
}
