using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class ReturnStep : Step
{
    public string Return
    {
        set => _status = value;
    }

    private string _status;
    
    public override Task<ObjectEntity> Execute(ObjectEntity state)
    {
        state.Properties.Add("status", new Entity { String = state.Substitute(_status) });
        state.Insert(new Entity { Boolean = true }, ApiOperation.FinalStateMarkLocation);
        return Task.FromResult(state);
    }
}