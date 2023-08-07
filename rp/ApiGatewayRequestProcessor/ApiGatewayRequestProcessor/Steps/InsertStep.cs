using ApiGatewayApi;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class InsertStep : Step
{
    public string Insert {get; set; }
    public string To { get; set; }
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        var value = state.Substitute(Insert);
        state.Insert(new Entity { String = value }, To);
        return state;
    }
}