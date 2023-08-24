using CCO.Entities;
using System.Text.Json;

namespace CCO.CCOConfigs
{
    public class CCOConfig
    {
        public DateTime ValidFrom { get; }
        public CCOIdentifier Id { get; }
        public Spec Data { get; }


        public CCOConfig(DateTime validFrom, string jsonString)
        {
            ValidFrom = validFrom;
            Data = ParseJsonString(jsonString);
            Id = new CCOIdentifier(Data.Title, Data.Version);
        }

        public bool IsActive(DateTime now)
        {
            return now >= ValidFrom;
        }

        public static Spec ParseJsonString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<Spec>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }
        public string GetDataString()
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(Data, options);
        }
    }
}
