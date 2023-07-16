namespace Configurator.Entities
{
    public class Config : ConfigId
    {
        public string Data { get; set; }

        public Config(string category, string apiName, string apiVersion, string data) 
            :base(category, apiName, apiVersion)
        {
            Data = data;
        }
    }

    public class ConfigId
    {
        public string Category { get; set; }
        public string ApiName { get; set; }
        public string ApiVersion { get; set; }

        public ConfigId(string category, string apiName, string apiVersion)
        {
            Category = category;
            ApiName = apiName;
            ApiVersion = apiVersion;
        }
    }
}
