using ApiGatewayApi;
using ApiGatewayRequestProcessor.ApiConfigs;
using ApiGatewayRequestProcessor.Gateways;
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
            { "break", typeof(BreakStep) },
            { "copy", typeof(CopyStep) },
            { "delete", typeof(DeleteStep) },
            { "foreach", typeof(ForeachStep) },
            { "http", typeof(HttpStep) },
            { "if", typeof(IfStep) },
            { "insert", typeof(InsertStep) },
            { "invoke", typeof(InvokeStep) },
            { "log", typeof(LogStep) },
            { "return", typeof(ReturnStep) },
            { "database", typeof(DatabaseStep) },
            { "cache", typeof(CacheStep) },
            { "queue", typeof(QueueStep) }
        };
        o.AddUniqueKeyTypeDiscriminator<Step>(keyMappings);
    };
    
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeDiscriminatingNodeDeserializer(TypeDiscriminator)
        .Build();
    
    public DateTime ValidFrom { get; }
    public string Data { get; }
    internal ApiConfig Config { get; }

    public ApiIdentifier Id { get;  }
    
    public ApiSpec(string data, DateTime validFrom)
    {
        ValidFrom = validFrom;
        Data = data;
        Config = YamlDeserializer.Deserialize<ApiConfig>(data);
        Id = new ApiIdentifier(Config.Name, Config.Version);
    }

    public bool HasOperation(string path, string method)
    {
        return Config.HasOperation(path, method);
    }

    public Task<ExecutionResponse> Execute(string path, string method, ExecutionRequest request, ApiGateway apiGateway,
        ConfigRepository repository, DateTime now)
    {
        Config.ResolveIncludes(repository, now);
        return Config.Execute(path, method, request, apiGateway);
    }
    
    public ApiMetadata GetMetadata()
    {
        return new ApiMetadata(Id.Name, Id.Version);
    }
    public bool IsActive(DateTime now)
    {
        return now >= ValidFrom;
    }

    protected bool Equals(ApiSpec other)
    {
        return ValidFrom.Equals(other.ValidFrom) && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ApiSpec)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ValidFrom, Id);
    }
}