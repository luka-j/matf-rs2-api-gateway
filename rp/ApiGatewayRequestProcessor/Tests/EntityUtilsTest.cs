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
}