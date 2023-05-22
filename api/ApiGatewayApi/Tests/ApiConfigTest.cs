using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Tests;

public class ApiConfigTest
{
    private const string GOOD_API_NAME = "Test spec 1";
    private const string GOOD_API_VERSION = "v1";
    private const string GOOD_API_URL = "http://localhost:8080";
    private const string GOOD_API_PATH = "/path";
    private const string GOOD_API_PATH_OP_SUMMARY = "Test op";
    
    private readonly OpenApiDocument _goodSpec, _incompleteSpec;
    
    public ApiConfigTest()
    {
        _goodSpec = new OpenApiDocument();
        _goodSpec.Info = new OpenApiInfo
        {
            Title = GOOD_API_NAME,
            Version = GOOD_API_VERSION
        };
        _goodSpec.Servers = new List<OpenApiServer>();
        _goodSpec.Servers.Add(new OpenApiServer
        {
            Url = GOOD_API_URL
        });
        _goodSpec.Paths = new OpenApiPaths();
        _goodSpec.Paths.Add(GOOD_API_PATH, new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {OperationType.Get , new OpenApiOperation
                {
                    Summary = GOOD_API_PATH_OP_SUMMARY, 
                    Responses = new OpenApiResponses { {"200", new OpenApiResponse() }}
                } }
            }
        });

        _incompleteSpec = new OpenApiDocument();
        _incompleteSpec.Info = new OpenApiInfo
        {
            Title = "Test spec incomplete"
        };
    }

    [Fact]
    public void GivenIncompleteSpec_WhenCreatingApiConfig_ThenThrowApiConfigException()
    {
        Assert.Throws<ApiConfigException>(() => new ApiConfig(_incompleteSpec, DateTime.Now - TimeSpan.FromMinutes(5)));
    }

    [Fact]
    public void GivenGoodSpec_WhenCreatingApiConfig_ThenCreateApiConfig()
    {
        var validFrom = DateTime.Now;
        var apiConfig = new ApiConfig(_goodSpec, validFrom);
        
        Assert.Equal(_goodSpec, apiConfig.Spec);
        Assert.Equal(validFrom, apiConfig.ValidFrom);
        Assert.Equal(new ApiIdentifier(GOOD_API_NAME, GOOD_API_VERSION), apiConfig.Id);
    }

    [Fact]
    public void GivenGoodSpec_WhenGettingMetadata_ThenReturnProperMetadata()
    {
        var validFrom = DateTime.Now;
        var apiConfig = new ApiConfig(_goodSpec, validFrom);

        var apiMetadata = apiConfig.GetMetadata();
        
        Assert.Equal(new ApiMetadata(GOOD_API_NAME, GOOD_API_VERSION, GOOD_API_URL), apiMetadata);
    }

    [Fact]
    public void GivenValidityTimeInTheFuture_WhenCheckingIsActive_ThenReturnFalse()
    {
        var now = DateTime.Now;
        var validFrom = DateTime.Now + TimeSpan.FromSeconds(1);
        var apiConfig = new ApiConfig(_goodSpec, validFrom);

        var isActive = apiConfig.IsActive(now);

        Assert.False(isActive);
    }

    [Fact]
    public void GivenValidityTimeNow_WhenCheckingIsActive_ThenReturnTrue()
    {
        var now = DateTime.Now;
        var apiConfig = new ApiConfig(_goodSpec, now);

        var isActive = apiConfig.IsActive(now);

        Assert.True(isActive);
    }

    [Fact]
    public void GivenExistingPathAndMethod_WhenResolvingOperation_ThenReturnOpenApiOperation()
    {
        var apiConfig = new ApiConfig(_goodSpec, DateTime.Now);

        var openApiOperation = apiConfig.ResolveOperation("Get", GOOD_API_PATH);
        
        Assert.NotNull(openApiOperation);
        Assert.Equal(GOOD_API_PATH_OP_SUMMARY, openApiOperation.Summary);
    }

    [Fact]
    public void GivenExistingPathAndWrongMethod_WhenResolvingOperation_ThenReturnNull()
    {
        var apiConfig = new ApiConfig(_goodSpec, DateTime.Now);

        var openApiOperation = apiConfig.ResolveOperation("Post", GOOD_API_PATH);
        
        Assert.Null(openApiOperation);
    }
    
    [Fact]
    public void GivenNonexistentPath_WhenResolvingOperation_ThenReturnNull()
    {
        var apiConfig = new ApiConfig(_goodSpec, DateTime.Now);

        var openApiOperation = apiConfig.ResolveOperation("Get", "/something");
        
        Assert.Null(openApiOperation);
    }

    [Fact]
    public void GivenExistingPathAndInvalidMethod_WhenResolvingOperation_ThenThrowApiRuntimeException()
    {
        var apiConfig = new ApiConfig(_goodSpec, DateTime.Now);

        Assert.Throws<ApiRuntimeException>(() => apiConfig.ResolveOperation("Fetch", GOOD_API_PATH));
    }

    [Fact]
    public void GivenGoodSpec_WhenGettingSpecString_ThenReturnStringWhichDeserializesToEquivalentSpec()
    {
        var apiConfig = new ApiConfig(_goodSpec, DateTime.Now);

        var specString = apiConfig.GetSpecString();
        
        var reader = new OpenApiStringReader();
        var doc = reader.Read(specString, out var diagnostic);
        Assert.Empty(diagnostic.Errors);
        Assert.Empty(diagnostic.Warnings);
        // Comparing OpenApiDocuments is awkward as it computes hashes during deserialization, which don't
        // match between invocations. So we're just comparing a few proxy values here.
        Assert.Equivalent(_goodSpec.Info, doc.Info);
        Assert.Single(doc.Paths);
    }
}