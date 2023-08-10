using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class CopyStepTest
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
    public async void GivenCopyStepWithSingleOperation_WhenExecuting_CopySingleResource()
    {
        var step = new CopyStep
        {
            Copy = "value -> copy"
        };
        var state = await step.Execute(_state, null);
        
        Assert.Equal("something", state.Find("${copy}")?.String);
    }

    [Fact]
    public async void GivenCopyStepWithMultipleOperations_WhenExecuting_CopyAllResources()
    {
        var step = new CopyStep
        {
            Copy = "${value} -> ${copy}, object.number -> integer"
        };
        var state = await step.Execute(_state, null);
        
        Assert.Equal("something", state.Find("${copy}")?.String);
        Assert.Equal(2, state.Find("${integer}")?.Integer);
    }

    [Fact]
    public async void GivenCopyStepWithMultipleDependentOperations_WhenExecuting_CopyAllResourcesInOrder()
    {
        var step = new CopyStep
        {
            Copy = "object -> ${another}, ${another} -> copy"
        };
        var state = await step.Execute(_state, null);
        
        Assert.Equal(2, state.Find("${copy.number}")?.Integer);
    }

    [Fact]
    public async void GivenCopyStepWithConstantDefaultAndMissingFrom_WhenExecuting_CopyDefaultValue()
    {
        var step = new CopyStep
        {
            Copy = "nothere / default -> copy"
        };
        var state = await step.Execute(_state, null);
        
        Assert.Equal("default", state.Find("${copy}")?.String);
    }
    
    [Fact]
    public async void GivenCopyStepWithVariableDefaultAndMissingFrom_WhenExecuting_CopyDefaultValue()
    {
        var step = new CopyStep
        {
            Copy = "nothere / ${object.number} -> copy"
        };
        var state = await step.Execute(_state, null);
        
        Assert.Equal(2, state.Find("${copy}")?.Integer);
    }

    [Fact]
    public async void GivenCopyStepWithMissingFromAndNoDefault_WhenExecuting_ThrowApiRuntimeException()
    {
        var step = new CopyStep
        {
            Copy = "nothere -> copy"
        };

        await Assert.ThrowsAsync<ApiRuntimeException>(async () => await step.Execute(_state, null));
    }
    
    [Fact]
    public async void GivenCopyStepWithMissingFromAndMissingDefault_WhenExecuting_ThrowApiRuntimeException()
    {
        var step = new CopyStep
        {
            Copy = "nothere / ${also.doesnt.exist} -> copy"
        };

        await Assert.ThrowsAsync<ApiRuntimeException>(async () => await step.Execute(_state, null));
    }
}