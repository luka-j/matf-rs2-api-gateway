namespace Configurator.CCOEntities
{
    public class CacheSpec
    {
        public string Title { get; set; }
        public CacheSource Cache { get; set; }

        public CacheSpec(string title, CacheSource cache)
        {
            Title = title;
            Cache = cache;
        }
    }
}
