using ApiGatewayRequestProcessor.ApiConfigs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiSpec
{
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)  // see height_in_inches in sample yml 
        .Build();
    
    public DateTime ValidFrom { get; }
    public string Data { get; }
    public ApiConfig Config { get;  }

    public ApiIdentifier Id { get;  }
    
    public ApiSpec(string data, DateTime validFrom)
    {
        ValidFrom = validFrom;
        Data = data;
        Config = YamlDeserializer.Deserialize<ApiConfig>(data);
        Id = new ApiIdentifier(Config.Name, Config.Version);
    }

    public ApiMetadata GetMetadata()
    {
        return new ApiMetadata(Id.Name, Id.Version);
    }
    public bool IsActive(DateTime now)
    {
        return now >= ValidFrom;
    }
}