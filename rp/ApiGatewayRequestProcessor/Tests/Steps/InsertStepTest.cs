using ApiGatewayApi;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class InsertStepTest
{
    private ObjectEntity _state = new()
    {
        Properties =
        {
            { "value", new Entity { String = "something" } },
            {
                "object", new Entity
                {
                    Object = new ObjectEntity
                    {
                        Properties =
                        {
                            { "number", new Entity { Integer = 2 } }
                        }
                    }
                }
            }
        }
    };

    [Fact]
    public async void GivenInsertStepWithLiteral_WhenExecuting_InsertLiteralToState()
    {
        var step = new InsertStep
        {
            Insert = "ins",
            To = "destination"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Equal("ins", state.Find("${destination}")?.String);
    }

    [Fact]
    public async void GivenInsertStepWithSimpleResource_WhenExecuting_InsertResourceValueToState()
    {
        var step = new InsertStep
        {
            Insert = "${object.number}",
            To = "destination"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Equal("2", state.Find("${destination}")?.String);
    }
    
    [Fact]
    public async void GivenInsertStepWithComplexResource_WhenExecuting_InsertResourceStringValueToState()
    {
        var step = new InsertStep
        {
            Insert = "${object}",
            To = "${destination}"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Equal("{number => 2}", state.Find("${destination}")?.String);
    }

    [Fact]
    public async void GivenInsertStepWithExpression_WhenExecuting_InsertSubstitutedExpressionToState()
    {
        var step = new InsertStep
        {
            Insert = "Value is ${value}, number is ${object.number}",
            To = "destination"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Equal("Value is something, number is 2", state.Find("${destination}")?.String);
    }
    
    [Fact]
    public async void GivenInsertStepWithNonexistentResource_WhenExecuting_InsertEmptyString()
    {
        var step = new InsertStep
        {
            Insert = "${nothere}",
            To = "${destination}"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Equal("", state.Find("${destination}")?.String);
    }
}