using CCO.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CCO.CCOConfigs
{
    public class CCOConfig
    {
        public DateTime ValidFrom { get; }
        public CCOIdentifier Id { get; }
        public Spec Data { get; }


        public CCOConfig(DateTime validFrom, string yamlString)
        {
            ValidFrom = validFrom;
            Data = ParseYamlString(yamlString);
            Id = new CCOIdentifier(Data.Title, Data.Version);
        }

        public bool IsActive(DateTime now)
        {
            return now >= ValidFrom;
        }

        public Spec ParseYamlString(string yamlString)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<Spec>(yamlString);
        }
        public string GetDataString()
        {
            var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return serializer.Serialize(Data);
        }
    }
}
