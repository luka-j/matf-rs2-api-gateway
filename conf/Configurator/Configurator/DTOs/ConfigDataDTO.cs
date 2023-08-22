namespace Configurator.DTOs
{
    public class ConfigDataDTO
    {
        public string Data { get; set; }
        public string ValidFrom { get; set; }

        public ConfigDataDTO(string data, string validFrom)
        {
            Data = data;
            ValidFrom = validFrom;
        }
    }
}
