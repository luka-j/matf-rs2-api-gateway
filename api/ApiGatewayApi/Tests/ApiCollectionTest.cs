using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;

namespace Tests;

public class ApiCollectionTest
{
  private const string SPEC_STRING = """
openapi: 3.0.1
info:
  title: test
  version: v1
servers:
  - url: http://localhost:8080
paths:
  /path:
    get:
      summary: Test op
      responses:
        '200':
          description: 
""";

  private readonly ApiCollection _apiCollection = new();

  [Fact]
  public void GivenApiSpecWithTooEarlyValidityStart_WhenAddingConfig_ThrowApiConfigException()
  {
    Assert.Throws<ApiConfigException>(() => _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, DateTime.Now)));
  }

  [Fact]
  public void GivenInvalidApiSpec_WhenAddingConfig_ThrowApiConfigException()
  {
    Assert.Throws<ApiConfigException>(() => _apiCollection.AddConfig(new ApiSpec("openapi: 3.0.1", DateTime.Now + TimeSpan.FromMinutes(1))));
  }

  [Fact]
  public void GivenValidApiSpec_WhenAddingConfig_ThenHasConfigReturnsTrue()
  {
    var configValidFrom = DateTime.Now + TimeSpan.FromMinutes(1);
    _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, configValidFrom));

    var hasConfig = _apiCollection.HasConfig(new ApiIdentifier("test", "v1"));
    
    Assert.True(hasConfig);
  }

  [Fact]
  public void GivenCollectionWithApiSpec_WhenGettingCurrentConfigBeforeSpecIsActive_ThenReturnNull()
  {
    var now = DateTime.Now;
    var configValidFrom = now + TimeSpan.FromMinutes(1);
    _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, configValidFrom));

    var currentConfig = _apiCollection.GetCurrentConfig(new ApiIdentifier("test", "v1"), now);

    Assert.Null(currentConfig);
  }

  [Fact]
  public void GivenCollectionWithMultipleVersionsOfApiConfig_WhenGettingCurrentConfig_ThenReturnProperSpec()
  {
    var now = DateTime.Now;
    var configValidFrom1 = now + TimeSpan.FromMinutes(1);
    var configValidFrom2 = now + TimeSpan.FromMinutes(2);
    _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, configValidFrom1));
    var v2Spec = SPEC_STRING.Replace("localhost", "127.0.0.1");
    _apiCollection.AddConfig(new ApiSpec(v2Spec, configValidFrom2));

    var currentConfig1 = _apiCollection.GetCurrentConfig(new ApiIdentifier("test", "v1"), now + TimeSpan.FromSeconds(90));
    var currentConfig2 = _apiCollection.GetCurrentConfig(new ApiIdentifier("test", "v1"), now + TimeSpan.FromSeconds(120));

    Assert.NotNull(currentConfig1);
    Assert.Equal("http://localhost:8080", currentConfig1.Spec.Servers[0].Url);
    Assert.NotNull(currentConfig2);
    Assert.Equal("http://127.0.0.1:8080", currentConfig2.Spec.Servers[0].Url);
  }
  
  [Fact]
  public void GivenCollectionWithMultipleConfigs_WhenGettingAllConfigMetadata_ThenReturnProperSpecMetadata()
  {
    var now = DateTime.Now;
    var configValidFrom1 = now + TimeSpan.FromMinutes(1);
    var configValidFrom2 = now + TimeSpan.FromMinutes(2);
    _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, configValidFrom1));
    var v2Spec = SPEC_STRING.Replace("v1", "v2");
    _apiCollection.AddConfig(new ApiSpec(v2Spec, configValidFrom1));
    var v3Spec = SPEC_STRING.Replace("v1", "v3");
    _apiCollection.AddConfig(new ApiSpec(v3Spec, configValidFrom2));

    var configMetadata = _apiCollection.GetAllConfigsMetadata(now + TimeSpan.FromSeconds(90));

    Assert.NotNull(configMetadata);
    Assert.Equal(2, configMetadata.Count());
    Assert.Contains(new ApiMetadata("test", "v1", "http://localhost:8080"), configMetadata);
    Assert.Contains(new ApiMetadata("test", "v2", "http://localhost:8080"), configMetadata);
  }

  [Fact]
  public void GivenCollectionWithMultipleVersionsOfConfig_WhenDeletingConfig_ThenDeleteAllVersions()
  {
    var now = DateTime.Now;
    var configValidFrom1 = now + TimeSpan.FromMinutes(1);
    var configValidFrom2 = now + TimeSpan.FromMinutes(2);
    _apiCollection.AddConfig(new ApiSpec(SPEC_STRING, configValidFrom1));
    var v2Spec = SPEC_STRING.Replace("localhost", "127.0.0.1");
    _apiCollection.AddConfig(new ApiSpec(v2Spec, configValidFrom2));

    var deleteConfig = _apiCollection.DeleteConfig(new ApiIdentifier("test", "v1"));
    var hasConfig = _apiCollection.HasConfig(new ApiIdentifier("test", "v1"));

    Assert.True(deleteConfig);
    Assert.False(hasConfig);
  }
}