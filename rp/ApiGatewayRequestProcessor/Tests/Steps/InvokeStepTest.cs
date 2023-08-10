using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class InvokeStepTest
{
    [Fact]
    public async void GivenInvokeStepWithoutStepRepository_WhenExecuting_ThrowApiRuntimeException()
    {
        var step = new InvokeStep
        {
            Invoke = "something"
        };

        await Assert.ThrowsAsync<ApiRuntimeException>(async () => await step.Execute(new ObjectEntity(), null));
    }
    
    [Fact]
    public async void GivenInvokeSteReferencingNonexistentStep_WhenExecuting_ThrowApiRuntimeException()
    {
        var step = new InvokeStep
        {
            Invoke = "something"
        };
        var stepRepository = new Dictionary<string, List<Step>>
        {
            { "step", new List<Step> { new CopyStep() } }
        };

        await Assert.ThrowsAsync<ApiRuntimeException>(async () => await step.Execute(new ObjectEntity(), stepRepository));
    }
    
    [Fact]
    public async void GivenInvokeStepWithProperStep_WhenExecuting_ThrowApiRuntimeException()
    {
        var step = new InvokeStep
        {
            Invoke = "step"
        };
        var state = new ObjectEntity();
        var stepRepository = new Dictionary<string, List<Step>>
        {
            { "step", new List<Step> { new InsertStep { Insert = "value", To = "dest" } } }
        };

        var result = await step.Execute(state, stepRepository);
        
        Assert.Equal("value", result.Find("${dest}")?.String);
    }
}