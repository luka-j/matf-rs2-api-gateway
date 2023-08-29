namespace Configurator.CCOEntities
{
    public class CacheSource
    {
        public string Type { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }

        public CacheSource(string type, string url, string password)
        {
            Type = type;
            Url = url;
            Password = password;
        }
    }
}
