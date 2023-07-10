using ApiGatewayApi;
using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Processing;
using Microsoft.OpenApi.Models;

namespace Tests.Processing;

public class RequestResponseFilterTest
{
    private readonly RequestResponseFilter _filter = new();
    
    private readonly Entity _complexEntity;

    public RequestResponseFilterTest()
    {
        _complexEntity = new Entity { Object = new ObjectEntity
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
    }

    [Fact]
    public void GivenComplexEntityAndPartiallyMatchingSpec_WhenFilteringBody_ReturnBodyMatchingSchema()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "test", new OpenApiSchema
                                    {
                                        Type = "number",
                                        Minimum = 5
                                    }
                                },
                                {
                                    "array", new OpenApiSchema
                                    {
                                        Type = "array",
                                        MaxItems = 1,
                                        MinItems = 1,
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                {
                                                    "flag", new OpenApiSchema
                                                    {
                                                        Type = "boolean"
                                                    }
                                                },
                                                {
                                                    "value", new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Pattern = "how.*\\?$"
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
            }
        };
        
        var expected = new Entity { Object = new ObjectEntity
            {
                Properties = 
                {
                    { "test", new Entity { Decimal = 5 } },
                    { "array", new Entity { List = new ListEntity {
                                Value = { new[] { new Entity
                                        {
                                            Object = new ObjectEntity {
                                                Properties =
                                                {
                                                    { "value", new Entity { String = "how are you?" } },
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

        var response = _filter.FilterBody(bodySpec, _complexEntity);
        Assert.Equal(expected, response);
    }
    
    [Fact]
    public void GivenComplexEntityAndPartiallyMatchingSpecWithStringValues_WhenFilteringBody_ReturnBodyMatchingSchema()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "test", new OpenApiSchema
                                    {
                                        Type = "string"
                                    }
                                },
                                {
                                    "array", new OpenApiSchema
                                    {
                                        Type = "array",
                                        MaxItems = 1,
                                        MinItems = 1,
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                {
                                                    "flag", new OpenApiSchema
                                                    {
                                                        Type = "string"
                                                    }
                                                },
                                                {
                                                    "value", new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Pattern = "how.*\\?$"
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
            }
        };
        
        var expected = new Entity { Object = new ObjectEntity
            {
                Properties = 
                {
                    { "test", new Entity { String = "5" } },
                    { "array", new Entity { List = new ListEntity {
                                Value = { new[] { new Entity
                                        {
                                            Object = new ObjectEntity {
                                                Properties =
                                                {
                                                    { "value", new Entity { String = "how are you?" } },
                                                    { "flag", new Entity { String = "False" } }
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

        var response = _filter.FilterBody(bodySpec, _complexEntity);
        Assert.Equal(expected, response);
    }

    [Fact]
    public void GivenEntityWhereStringFieldShouldBeParsed_WhenFilteringBody_StringShouldBeReturnedParsed()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "parse", new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Maximum = 1,
                                        Minimum = 1
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        
        var expected = new Entity { Object = new ObjectEntity
            {
                Properties = 
                {
                    { "parse", new Entity { Integer = 1 } }
                }
            }
        };

        var response = _filter.FilterBody(bodySpec, _complexEntity);
        Assert.Equal(expected, response);
    }

    [Fact]
    public void GivenComplexEntityWithMissingRequiredFields_WhenFilteringBody_ThrowValidationException()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "value", new OpenApiSchema
                                    {
                                        Type = "number",
                                        Minimum = 5
                                    }
                                }
                            },
                            Required = { "value" }
                        }
                    }
                }
            }
        };

        Assert.Throws<ParamValidationException>(() => _filter.FilterBody(bodySpec, _complexEntity));
    }
    
    [Fact]
    public void GivenComplexEntityWithFailingListValidation_WhenFilteringBody_ThrowValidationException()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "array", new OpenApiSchema
                                    {
                                        Type = "array",
                                        MinItems = 3,
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                {
                                                    "flag", new OpenApiSchema
                                                    {
                                                        Type = "boolean"
                                                    }
                                                },
                                                {
                                                    "value", new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Pattern = "how.*\\?$"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Required = { "value" }
                        }
                    }
                }
            }
        };

        Assert.Throws<ParamValidationException>(() => _filter.FilterBody(bodySpec, _complexEntity));
    }
    
    [Fact]
    public void GivenComplexEntityWithFailingStringPattern_WhenFilteringBody_ThrowValidationException()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "array", new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                {
                                                    "flag", new OpenApiSchema
                                                    {
                                                        Type = "boolean"
                                                    }
                                                },
                                                {
                                                    "value", new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Pattern = "hey.*"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Required = { "value" }
                        }
                    }
                }
            }
        };

        Assert.Throws<ParamValidationException>(() => _filter.FilterBody(bodySpec, _complexEntity));
    }
    
    [Fact]
    public void GivenComplexEntityWithFailingNumberValidation_WhenFilteringBody_ThrowValidationException()
    {
        var bodySpec = new OpenApiRequestBody
        {
            Content =
            {
                {
                    "application/json", new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                {
                                    "array", new OpenApiSchema
                                    {
                                        Type = "array",
                                        Items = new OpenApiSchema
                                        {
                                            Type = "object",
                                            Properties =
                                            {
                                                {
                                                    "number", new OpenApiSchema
                                                    {
                                                        Type = "integer",
                                                        Maximum = 2,
                                                        ExclusiveMaximum = true
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Required = { "value" }
                        }
                    }
                }
            }
        };

        Assert.Throws<ParamValidationException>(() => _filter.FilterBody(bodySpec, _complexEntity));
    }

    [Fact]
    public void GivenComplexParams_WhenFilteringParams_ReturnFilteredResponse()
    {
        var queryParams = new PrimitiveOrListObjectEntity
        {
            Properties =
            {
                {
                    "q", new PrimitiveOrList
                    {
                        List = new PrimitiveList
                        {
                            Value =
                            {
                                new[] { new PrimitiveEntity { String = "test" }, new PrimitiveEntity { String = "2" } }
                            }
                        }
                    }
                },
                { "qp", new PrimitiveOrList { Primitive = new PrimitiveEntity { Integer = 4 } } }
            }
        };
        var headerParams = new PrimitiveOrListObjectEntity
        {
            Properties =
            {
                {
                    "id", new PrimitiveOrList { Primitive = new PrimitiveEntity { String = "test" } }
                },
                {
                    "hp", new PrimitiveOrList { Primitive = new PrimitiveEntity { Boolean = false } }
                }
            }
        };
        var pathParams = new PrimitiveObjectEntity
        {
            Properties =
            {
                { "sid", new PrimitiveEntity { Integer = 1 } }
            }
        };
        
        var requestParams = new List<OpenApiParameter>
        {
            new()
            {
                Name = "q",
                Required = true,
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        MaxLength = 5
                    }
                },
            },
            new()
            {
                Name = "qp",
                In = ParameterLocation.Query,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            },
            new()
            {
                Name = "hp",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "integer"
                }
            },
            new()
            {
                Name = "optional",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            },
            new()
            {
                Name = "sid",
                In = ParameterLocation.Path,
                Schema = new OpenApiSchema
                {
                    Type = "number"
                }
            }
        };
        
        var queryParamsExpected = new PrimitiveOrListObjectEntity
        {
            Properties =
            {
                {
                    "q", new PrimitiveOrList
                    {
                        List = new PrimitiveList
                        {
                            Value =
                            {
                                new[] { new PrimitiveEntity { String = "test" }, new PrimitiveEntity { String = "2" } }
                            }
                        }
                    }
                },
                { "qp", new PrimitiveOrList { Primitive = new PrimitiveEntity { String = "4" } } }
            }
        };
        var headerParamsExpected = new PrimitiveOrListObjectEntity
        {
            Properties =
            {
                {
                    "hp", new PrimitiveOrList { Primitive = new PrimitiveEntity { Integer = 0 } }
                }
            }
        };
        var pathParamsExpected = new PrimitiveObjectEntity
        {
            Properties =
            {
                { "sid", new PrimitiveEntity { Decimal = 1 } }
            }
        };

        var filteredParams = _filter.FilterParams(requestParams, pathParams, headerParams, queryParams);
        Assert.Equal(Tuple.Create(pathParamsExpected, headerParamsExpected, queryParamsExpected), filteredParams);
    }
}