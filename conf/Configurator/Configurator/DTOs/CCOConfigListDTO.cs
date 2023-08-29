using Configurator.CCOEntities;

namespace Configurator.DTOs
{
    public class CCOConfigListDTO
    {
        public IEnumerable<DatabaseSpec> Databases { get; set; }
        public IEnumerable<CacheSpec> Caches { get; set; }
        public IEnumerable<QueueSpec> Queues { get; set; }

        public CCOConfigListDTO(IEnumerable<DatabaseSpec> databases, IEnumerable<CacheSpec> caches, IEnumerable<QueueSpec> queues)
        {
            Databases = databases;
            Caches = caches;
            Queues = queues;
        }
    }
}
