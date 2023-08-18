using CCO.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CCO.CCOConfigs
{
    public class CCOConfig
    {
        public DateTime ValidFrom { get; }
        public CCOIdentifier Id { get; }
        public YamlResponse YamlResponseData { get; }


        public CCOConfig(DateTime validFrom, string yamlString)
        {
            ValidFrom = validFrom;
            YamlResponseData = ParseYamlString(yamlString);
            Id = new CCOIdentifier(YamlResponseData.Title, YamlResponseData.Version);

            Console.WriteLine($"Name: {YamlResponseData.Databases.ElementAt(1).Name}");
        }

        public bool IsActive(DateTime now)
        {
            return now >= ValidFrom;
        }

        public YamlResponse ParseYamlString(string yamlString)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<YamlResponse>(yamlString);
        }
    }
}
