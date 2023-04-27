using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace ApiGatewayApi.ApiConfigs;

public class ApiConfig
{
    public DateTime ValidFrom { get; }
    public OpenApiDocument Spec { get; }
    public ApiIdentifier Id { get;  }
    
    private readonly PathFragmentTree _pathTree;
    private static readonly List<KeyValuePair<Func<OpenApiDocument, bool>, ApiConfigException>> SpecValidators = InitializeValidators();

    public ApiConfig(OpenApiDocument spec, DateTime validFrom)
    {
        ValidateConfig(spec);
        ValidFrom = validFrom;
        Spec = spec;
        Id = new ApiIdentifier(spec.Info.Title, spec.Info.Version);
        _pathTree = PathFragmentTree.BuildTree(spec.Paths);
    }

    private static List<KeyValuePair<Func<OpenApiDocument, bool>, ApiConfigException>> InitializeValidators()
    {
        var validators = new List<KeyValuePair<Func<OpenApiDocument, bool>, ApiConfigException>>();
        validators.Add(new(spec => spec.Info == null,
            new ApiConfigException("ApiConfig info is null!")));
        validators.Add(new(spec => spec.Info.Title == null || spec.Info.Version == null, 
            new ApiConfigException("ApiConfig info is not populated properly!")));
        validators.Add(new(spec => spec.Servers == null, 
            new ApiConfigException("ApiConfig servers is null!")));
        validators.Add(new(spec => spec.Servers.Count != 1, 
            new ApiConfigException("We currently support only a single server entry.")));
        validators.Add(new(spec => spec.Servers[0].Url == null, 
            new ApiConfigException("ApiConfig server.url must be populated")));
        return validators;
    }

    private void ValidateConfig(OpenApiDocument spec)
    {
        foreach (var validator in SpecValidators.Where(validator => validator.Key.Invoke(spec)))
        {
            throw validator.Value;
        }
    }

    public bool IsActive(DateTime now)
    {
        return now >= ValidFrom;
    }

    public OpenApiOperation? ResolveOperation(string method, string path)
    {
        var pathItem = _pathTree.ResolvePath(path);
        if (pathItem == null) return null;

        if (!Enum.TryParse(method, out OperationType op))
        {
            throw new ApiRuntimeException(method + " is not a valid method!");
        }

        return pathItem.Operations[op];
    }

    public string GetSpecString()
    {
        var stringWriter = new StringWriter();
        var yamlWriter = new OpenApiYamlWriter(stringWriter);
        Spec.SerializeAsV3(yamlWriter);
        return stringWriter.ToString();
    }

    public ApiMetadata GetMetadata()
    {
        return new ApiMetadata(Spec.Info.Title, Spec.Info.Version, Spec.Servers[0].Url);
    }
}