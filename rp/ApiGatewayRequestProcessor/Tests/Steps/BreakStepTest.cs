using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class BreakStepTest
{
    [Fact]
    public async void GivenEmptyBreakStep_WhenExecuting_PopulateOnlyBreakMark()
    {
        var step = new BreakStep();
        var state = new ObjectEntity();

        state = await step.Execute(state, null);
        
        Assert.True(state.Find(ApiOperation.BreakMarkLocation)?.Boolean);
    }
    
    [Fact]
    public async void GivenBreakStepWithStatus_WhenExecuting_PopulateBreakMarkAndStatus()
    {
        var step = new BreakStep
        {
            Break = "200"
        };
        var state = new ObjectEntity();

        state = await step.Execute(state, null);
        
        Assert.True(state.Find(ApiOperation.BreakMarkLocation)?.Boolean);
        Assert.Equal("200", state.Find(ApiOperation.StatusLocation)?.String);
    }
}