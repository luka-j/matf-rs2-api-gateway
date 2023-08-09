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
    
    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        state.Insert(new Entity { String = state.Substitute(_status) }, ApiOperation.StatusLocation);
        state.Insert(new Entity { Boolean = true }, ApiOperation.FinalStateMarkLocation);
        return Task.FromResult(state);
    }
}