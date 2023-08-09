using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class BreakStep : Step
{
    public string? Break { get; set; }
    
    public override Task<ObjectEntity> Execute(ObjectEntity state)
    {
        if (Break != null && Break.Trim().Length > 0)
        {
            state.Insert(new Entity { String = Break}, ApiOperation.StatusLocation);
        }
        
        state.Insert(new Entity { Boolean = true }, ApiOperation.BreakMarkLocation);
        return Task.FromResult(state);
    }
}