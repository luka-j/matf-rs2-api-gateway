using System.Text.Json.Nodes;
using ApiGatewayApi;
using ApiGatewayApi.Processing;

namespace Tests;

public class EntityMapperTest
{
    private readonly EntityMapper _mapper = new();

    private readonly JsonNode _complexNode;
    private readonly Entity _complexEntity;

    public EntityMapperTest()
    {
        _complexNode = JsonNode.Parse("{\"test\": 5, \"array\": [{\"value\": \"how are you?\", \"flag\": false}]}")!;
        _complexEntity = new Entity();
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
        _complexEntity.Object = objectEntity;
    }
    
    [Fact]
    public void GivenComplexObject_WhenMappingToEntity_ReturnProperlyParsedEntity()
    {
        var result = _mapper.MapToEntity(_complexNode);
        Assert.Equal(_complexEntity, result);
    }

    [Fact]
    public void GivenComplexObject_WhenMappingToJsonNodeAndBack_ReturnEqualResultAsOriginal()
    {
        var result = _mapper.MapToJsonNode(_complexEntity).ToJsonString();
        var original = _mapper.MapToEntity(JsonNode.Parse(result));
        Assert.Equal(_complexEntity, original);
    }
}