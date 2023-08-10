using ApiGatewayApi;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class DeleteStepTest
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
    public async void GivenDeleteStepWithSingleTarget_WhenExecuting_DeleteResourceAtTarget()
    {
        var step = new DeleteStep
        {
            Delete = "value"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Null(state.Find("value"));
    }
    
    [Fact]
    public async void GivenDeleteStepWithMultipleTargets_WhenExecuting_DeleteAllResources()
    {
        var step = new DeleteStep
        {
            Delete = "value, object.number"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Null(state.Find("value"));
        Assert.Null(state.Find("object.number"));
    }
    
    [Fact]
    public async void GivenDeleteStepSomeNonexistentTargets_WhenExecuting_DeleteAllExistingTargets()
    {
        var step = new DeleteStep
        {
            Delete = "value, something, else"
        };

        var state = await step.Execute(_state, null);
        
        Assert.Null(state.Find("value"));
    }
}