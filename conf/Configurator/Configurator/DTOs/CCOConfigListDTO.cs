using Configurator.Entities;

namespace Configurator.DTOs
{
    public class CCOConfigListDTO
    {
        public IEnumerable<CCOSpec> Databases { get; set; }
        public IEnumerable<CCOSpec> Caches { get; set; }
        public IEnumerable<CCOSpec> Queues { get; set; }

        public CCOConfigListDTO(IEnumerable<CCOSpec> databases, IEnumerable<CCOSpec> caches, IEnumerable<CCOSpec> queues)
        {
            Databases = databases;
            Caches = caches;
            Queues = queues;
        }
    }
}
