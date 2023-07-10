using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace ApiGatewayApi.ApiConfigs;

/// <summary>
/// Represents API configuration, holding api spec, identifier and validity start time.
/// </summary>
public class ApiConfig
{
    public DateTime ValidFrom { get; }
    public OpenApiDocument Spec { get; }
    public ApiIdentifier Id { get; }
    
    private readonly PathSegmentTree _pathTree;
    private static readonly List<KeyValuePair<Func<OpenApiDocument, bool>, ApiConfigException>> SpecValidators = InitializeValidators();

    /// <summary>
    /// Create new API config based on OpenApiDocument spec with given validity start time.
    /// </summary>
    /// <param name="spec">OpenApiDocument representing API spec. It must contain basic metadata (title, version,
    /// a single server and its URL).</param>
    /// <param name="validFrom">Validity start time for this config. This config should not be used before its
    /// validity start.</param>
    public ApiConfig(OpenApiDocument spec, DateTime validFrom)
    {
        ValidateConfig(spec);
        ValidFrom = validFrom;
        Spec = spec;
        Id = new ApiIdentifier(spec.Info.Title, spec.Info.Version);
        _pathTree = PathSegmentTree.BuildTree(spec.Paths);
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
        validators.Add(new(spec => spec.Paths == null, 
            new ApiConfigException("Paths must be defined")));
        return validators;
    }

    private void ValidateConfig(OpenApiDocument spec)
    {
        foreach (var validator in SpecValidators.Where(validator => validator.Key.Invoke(spec)))
        {
            throw validator.Value;
        }
    }

    /// <summary>
    /// Check whether this API Config is active. If this method returns false, no other operations should be
    /// performed on this ApiConfig object.
    /// </summary>
    /// <param name="now">Current time</param>
    /// <returns>Whether this API config is active. This method returning true doesn't guarantee this config
    /// should be used (e.g. maybe there's another config which is also active and newer), i.e. it doesn't
    /// guarantee <i>validity</i>.</returns>
    public bool IsActive(DateTime now)
    {
        return now >= ValidFrom;
    }

    /// <summary>
    /// Resolve OpenApiOperation on this config, given HTTP method and URL path.
    /// </summary>
    /// <param name="method">HTTP method of the request.</param>
    /// <param name="path">Path part of the URL (excluding query params and fragment).</param>
    /// <returns>OpenApiOperation if it exists, null if it doesn't.</returns>
    /// <exception cref="ApiRuntimeException">If passed method isn't a HTTP request method.</exception>
    public KeyValuePair<string, OpenApiOperation>? ResolveOperation(string method, string path)
    {
        var pathItem = _pathTree.ResolvePath(path);
        if (pathItem == null) return null;

        var capitalizedMethod = char.ToUpper(method[0]) + method[1..].ToLower();
        if (!Enum.TryParse(capitalizedMethod, out OperationType op))
        {
            throw new ApiRuntimeException(method + " is not a valid method!");
        }

        if (!pathItem.Item.Operations.ContainsKey(op)) return null;
        return new KeyValuePair<string, OpenApiOperation>(pathItem.SpecPath, pathItem.Item.Operations[op]);
    }

    /// <summary>
    /// Serialize spec of this config to string.
    /// </summary>
    /// <returns>API Spec in OAS3 YAML format.</returns>
    public string GetSpecString()
    {
        var stringWriter = new StringWriter();
        var yamlWriter = new OpenApiYamlWriter(stringWriter);
        Spec.SerializeAsV3(yamlWriter);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Get metadata of this API Config.
    /// </summary>
    /// <returns>An ApiMetadata object corresponding to this ApiConfig.</returns>
    public ApiMetadata GetMetadata()
    {
        return new ApiMetadata(Spec.Info.Title, Spec.Info.Version, Spec.Servers[0].Url);
    }
}