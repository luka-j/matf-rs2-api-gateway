using ApiGatewayApi;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class InsertStep : Step
{
    public string Insert {get; set; }
    public string To { get; set; }
    
    public override Task<ObjectEntity> Execute(ObjectEntity state)
    {
        var value = state.Substitute(Insert);
        state.Insert(new Entity { String = value }, To);
        return Task.FromResult(state);
    }
}