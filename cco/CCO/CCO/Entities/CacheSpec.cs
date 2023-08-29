namespace CCO.Entities
{
    public class CacheSpec : ISpec
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
