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
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        state.Properties.Add("status", new Entity { String = _status });
        state.Insert(new Entity { Boolean = true }, ApiOperation.FinalStateMarkLocation);
        return state;
    }
}