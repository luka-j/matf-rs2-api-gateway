namespace Configurator.DTOs
{
    public class ConfigPostDTO
    {
        public string ApiName { get; set; }
        public string ApiVersion { get; set; }
        public string Data { get; set; }

        public ConfigPostDTO(string apiName, string apiVersion, string data)
        {
            ApiName = apiName;
            ApiVersion = apiVersion;
            Data = data;
        }
    }
}
