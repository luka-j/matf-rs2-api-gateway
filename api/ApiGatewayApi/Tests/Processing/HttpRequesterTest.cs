using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ApiGatewayApi;
using ApiGatewayApi.Processing;

namespace Tests.Processing;

public class HttpRequesterTest
{
    private readonly ApiGatewayApi.Processing.HttpRequester _requester = new(new RequestResponseFilter(), new EntityMapper(), 
        null, null);
    
    [Fact]
    public void GivenSimpleGetRequest_WhenMakingRequestMessage_MakeProperMessage()
    {
        var result = _requester.MakeHttpRequestMessage("GET", "http://example.com/test", 
            null, null, null, null);
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://example.com/test")
        };
        Assert.Equal(expected.ToString(), result.ToString()); // HttpRequestMessage doesn't have a sane equals
    }
    
    [Fact]
    public void GivenGetRequestWithPathParams_WhenMakingRequestMessage_BuildProperUri()
    {
        var result = _requester.MakeHttpRequestMessage("GET", "http://example.com/test/{param}/something", 
            null, new PrimitiveObjectEntity
            {
                Properties = { {"param", new PrimitiveEntity {String = "value"} } }
            }, null, null);
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://example.com/test/value/something")
        };
        Assert.Equal(expected.ToString(), result.ToString());
    }

    [Fact]
    public void GivenGetRequestWithQueryParams_WhenMakingRequestMessage_IncludeQueryParamsInUrl()
    {
        var result = _requester.MakeHttpRequestMessage("GET", "http://example.com/test", 
            null, null, null, new PrimitiveOrListObjectEntity
            {
                Properties =
                {
                    {"param", new PrimitiveOrList { Primitive = new PrimitiveEntity {String = "value"}}},
                    {"list", new PrimitiveOrList {List = new PrimitiveList {Value = { new PrimitiveEntity[]{new() {String = "1"}, new() {String = "2"}} }}}}
                }
            });
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://example.com/test?param=value&list=1&list=2")
        };
        Assert.Equal(expected.ToString(), result.ToString());
    }

    [Fact]
    public void GivenGetRequestWithHeaders_WhenMakingRequestMessage_IncludeHeadersInMessage()
    {
        var result = _requester.MakeHttpRequestMessage("GET", "http://example.com/test", 
            null, null, new PrimitiveOrListObjectEntity
            {
                Properties =
                {
                    {"param", new PrimitiveOrList { Primitive = new PrimitiveEntity {String = "value"}}},
                    {"list", new PrimitiveOrList {List = new PrimitiveList {Value = { new PrimitiveEntity[]{new() {String = "1"}, new() {String = "2"}} }}}}
                }
            }, null);
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("http://example.com/test"),
            Headers =
            {
                {"param", "value"},
                {"list", new[] {"1", "2"}}
            }
        };
        Assert.Equal(expected.ToString(), result.ToString());
    }

    [Fact]
    public void GivenPostRequestWithBody_WhenMakingRequestMessage_PopulateContentProperly()
    {
        var requestBody = new Entity();
        var objectEntity = new ObjectEntity();
        var childTest = new Entity();
        childTest.Decimal = (decimal)5;
        objectEntity.Properties["test"] = childTest;
        var childArray = new Entity();
        var listEntity = new ListEntity();
        var listMember = new ObjectEntity();
        var listMemberValue = new Entity();
        listMemberValue.String = "how are you?";
        listMember.Properties["value"] = listMemberValue;
        var listMemberFlag = new Entity();
        listMemberFlag.Boolean = false;
        listMember.Properties["flag"] = listMemberFlag;
        listEntity.Value.Add(new Entity {Object = listMember});
        childArray.List = listEntity;
        objectEntity.Properties["array"] = childArray;
        requestBody.Object = objectEntity;

        var result = _requester.MakeHttpRequestMessage("POST", "http://example.com/value",
            requestBody, null, null, null);
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("http://example.com/value"),
            Content = JsonContent.Create(JsonNode.Parse("{\"test\": 5, \"array\": [{\"value\": \"how are you?\", \"flag\": false}]}")),
        };
        Assert.Equal(expected.ToString(), result.ToString());
    }

    [Fact]
    public void GivenPutRequestWithAllParams_WhenMakingRequestMessage_ReturnProperlyFilledMessage()
    {
        var result = _requester.MakeHttpRequestMessage("PUT", "http://example.com/{param}/{other}/stuff", 
            null, new PrimitiveObjectEntity
            {
                Properties =
                {
                    {"param", new PrimitiveEntity {String = "val1"}},
                    {"other", new PrimitiveEntity {Integer = 2}}
                }
            }, new PrimitiveOrListObjectEntity
            {
                Properties =
                {
                    {"param", new PrimitiveOrList { Primitive = new PrimitiveEntity {String = "value"}}},
                    {"list", new PrimitiveOrList {List = new PrimitiveList {Value = { new PrimitiveEntity[]{new() {String = "1"}, new() {String = "2"}} }}}}
                }
            }, new PrimitiveOrListObjectEntity
            {
                Properties =
                {
                    {"q", new PrimitiveOrList { Primitive = new PrimitiveEntity {String = "test"}}},
                    {"s", new PrimitiveOrList {List = new PrimitiveList {Value = { new PrimitiveEntity[]{new() {String = "2"}, new() {String = "2"}} }}}}
                }
            });
        var expected = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri("http://example.com/val1/2/stuff?q=test&s=2&s=2"),
            Headers =
            {
                {"param", "value"},
                {"list", new[] {"1", "2"}}
            }
        };
        Assert.Equal(expected.ToString(), result.ToString());
    }
}