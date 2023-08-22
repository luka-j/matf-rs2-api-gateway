namespace Configurator.DTOs
{
    public class ConfigMetadataDTO
    {
        public string ApiName { get; set; }
        public string ApiVersion { get; set; }
        public string? BasePath { get; set; }
        public ConfigMetadataDTO(string apiName, string apiVersion, string? basePath)
        {
            ApiName = apiName;
            ApiVersion = apiVersion;
            BasePath = basePath;
        }
    }
}
