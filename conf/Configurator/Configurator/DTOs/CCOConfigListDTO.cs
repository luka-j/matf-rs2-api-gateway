using Configurator.CCOEntities;

namespace Configurator.DTOs
{
    public class CCOConfigListDTO
    {
        public IEnumerable<CCOSpec<DatabaseSource>> Databases { get; set; }
        public IEnumerable<CCOSpec<CacheSource>> Caches { get; set; }
        public IEnumerable<CCOSpec<QueueSource>> Queues { get; set; }

        public CCOConfigListDTO(IEnumerable<CCOSpec<DatabaseSource>> databases, IEnumerable<CCOSpec<CacheSource>> caches, IEnumerable<CCOSpec<QueueSource>> queues)
        {
            Databases = databases;
            Caches = caches;
            Queues = queues;
        }
    }
}
