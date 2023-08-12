using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace Tests.Steps;

public class ReturnStepTest
{
    
    [Fact]
    public async void GivenReturnStepWithStatusOnly_WhenExecuting_PopulateReturnMarkAndStatus()
    {
        var step = new ReturnStep
        {
            Return = "200"
        };
        var state = new ObjectEntity();

        state = await step.Execute(state, null);
        
        Assert.True(state.Find(ApiOperation.FinalStateMarkLocation)?.Boolean);
        Assert.Equal("200", state.Find(ApiOperation.StatusLocation)?.String);
    }

    [Fact]
    public async void
        GivenReturnStepWithStatusAndLocation_WhenExecuting_PopulateReturnMarkAndStatusAndReturnEntityOnLocation()
    {
        var step = new ReturnStep
        {
            Return = "200 ${result}"
        };
        var state = new ObjectEntity
        {
            Properties =
            {
                {
                    "result", new Entity
                    {
                        Object = new ObjectEntity
                        {
                            Properties =
                            {
                                { "value", new Entity { String = "5" } }
                            }
                        }
                    }
                }
            }
        };

        state = await step.Execute(state, null);
        
        Assert.True(state.Find(ApiOperation.FinalStateMarkLocation)?.Boolean);
        Assert.Equal("200", state.Find(ApiOperation.StatusLocation)?.String);
        Assert.Equal("5", state.Find("${value}")?.String);
    }
}