using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;

namespace Tests;

public class EntityUtilsTest
{
    private readonly Entity _complexEntity = new()
    { Object = new ObjectEntity
        {
            Properties = 
            {
                { "test", new Entity { Decimal = 5 } },
                { "parse", new Entity { String = "1" }},
                { "array", new Entity { List = new ListEntity {
                            Value = { new[] { new Entity
                                    {
                                        Object = new ObjectEntity {
                                            Properties =
                                            {
                                                { "value", new Entity { String = "how are you?" } },
                                                { "number", new Entity { Integer = 2 }},
                                                { "flag", new Entity { Boolean = false } }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    };
       
    
    
    [Fact]
    public void GivenComplexEntity_WhenFindValueInNestedList_ThenReturnProperValueEntity()
    {
        var result = _complexEntity.Object.Find("${array[0].value}");
        Assert.NotNull(result);
        Assert.Equal(new Entity { String = "how are you?"}, result);
    }

    [Fact]
    public void GivenComplexEntity_WhenDoingSimpleFind_ReturnProperObject()
    {
        var result = _complexEntity.Object.Find("${array}");
        Assert.NotNull(result);
        Assert.Single(result.List.Value);
        Assert.Equal(2, result.List.Value[0].Object.Properties["number"].Integer);
    }

    [Fact]
    public void GivenComplexEntity_WhenTryingToFindNonexistentLocation_ReturnNull()
    {
        var result = _complexEntity.Object.Find("${array[0].something}");
        Assert.Null(result);
    }

    [Fact]
    public void GivenComplexEntity_WhenTryingToFindIndexOutOfBounds_ReturnNull()
    {
        var result = _complexEntity.Object.Find("${array[1]}");
        Assert.Null(result);
    }

    [Fact]
    public void GivenComplexObject_WhenDoingSimpleInsert_InsertToProperPlace()
    {
        var e = new Entity { String = "success" };
        _complexEntity.Object.Insert(e, "${value}");
        
        Assert.Equal(e, _complexEntity.Object.Properties["value"]);
    }

    [Fact]
    public void GivenEmptyObject_WhenInsertingValue_InsertProperValues()
    {
        var obj = new ObjectEntity();
        var value = new Entity{ Object = new ObjectEntity
            {
                Properties =
                {
                    { "test", new Entity { String = "value" } }
                }
            }
        };

        obj.Insert(value, "some.random[*].path");
        
        Assert.Equal("value", obj.Properties["some"].Object
            .Properties["random"].List
            .Value[0].Object
            .Properties["path"].Object
            .Properties["test"].String);
    }

    [Fact]
    public void GivenComplexObject_WhenDoingNestedInsert_CreateObjectsAndInsertToProperPlace()
    {
        var e = new Entity { String = "success" };
        _complexEntity.Object.Insert(e, "${some.long.place}");
        
        Assert.Equal(e, _complexEntity.Object.Properties["some"]
            .Object.Properties["long"]
            .Object.Properties["place"]);
    }

    [Fact]
    public void GivenComplexObject_WhenInsertingToList_CreateNewElementInList()
    {
        var e = new Entity { String = "success" };
        _complexEntity.Object.Insert(e, "${array[*]}");
            
        Assert.Equal(e, _complexEntity.Object.Properties["array"].List.Value[1]);
    }

    [Fact]
    public void GivenComplexObject_WhenInsertingToObjectInList_InsertEntityToObject()
    {
        var e = new Entity { String = "success" };
        _complexEntity.Object.Insert(e, "${array[0].test}");
            
        Assert.Equal(e, _complexEntity.Object.Properties["array"].List.Value[0].Object.Properties["test"]);
    }

    [Fact]
    public void GivenComplexObject_WhenInsertingToExistingListIndex_OverwriteEntity()
    {
        var e = new Entity { String = "success" };
        _complexEntity.Object.Insert(e, "${array[0]}");
        
        Assert.Equal(e, _complexEntity.Object.Properties["array"].List.Value[0]);
    }

    [Fact]
    public void GivenComplexObject_WhenInsertingToListIndexOutOfBounds_ThrowException()
    {
        var e = new Entity { String = "success" };
        Assert.Throws<ApiRuntimeException>(() => _complexEntity.Object.Insert(e, "${array[1]}"));
    }

    [Fact]
    public void GivenComplexObject_WhenSubstitutingIntoAString_ReturnProperString()
    {
        var result = _complexEntity.Object.Substitute("Hi ${test}, ${array[0].value}");
        Assert.Equal("Hi 5, how are you?", result);
    }

    [Fact]
    public void GivenComplexObject_WhenDeletingSimpleProperty_RemoveItFromObject()
    {
        _complexEntity.Object.Delete("test");
        Assert.False(_complexEntity.Object.Properties.ContainsKey("test"));
    }
    
    [Fact]
    public void GivenComplexObject_WhenDeletingWholeList_RemoveItFromObject()
    {
        _complexEntity.Object.Delete("array");
        Assert.False(_complexEntity.Object.Properties.ContainsKey("array"));
    }

    [Fact]
    public void GivenComplexObject_WhenDeletingItemFromList_RemoveItFromList()
    {
        _complexEntity.Object.Delete("array[0]");
        Assert.Empty(_complexEntity.Object.Properties["array"].List.Value);
    }

    [Fact]
    public void GivenComplexObject_WhenDeletingANestedField_RemoveIt()
    {
        _complexEntity.Object.Delete("array[0].number");
        Assert.False(_complexEntity.Object.Properties["array"].List.Value[0].Object.Properties.ContainsKey("number"));
    }
}