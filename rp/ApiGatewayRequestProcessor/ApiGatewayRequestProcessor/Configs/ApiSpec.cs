using ApiGatewayRequestProcessor.ApiConfigs;
using ApiGatewayRequestProcessor.Steps;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.BufferedDeserialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiSpec
{
    private static readonly Action<ITypeDiscriminatingNodeDeserializerOptions> TypeDiscriminator = o =>
    {
        var keyMappings = new Dictionary<string, Type>
        {
            { "copy", typeof(CopyStep) },
            { "delete", typeof(DeleteStep) },
            { "foreach", typeof(ForeachStep) },
            { "http", typeof(HttpStep) },
            { "if", typeof(IfStep) },
            { "insert", typeof(InsertStep) },
            { "log", typeof(LogStep) },
            { "return", typeof(ReturnStep) },
        };
        o.AddUniqueKeyTypeDiscriminator<Step>(keyMappings);
    };
    
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeDiscriminatingNodeDeserializer(TypeDiscriminator)
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