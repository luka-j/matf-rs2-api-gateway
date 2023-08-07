using ApiGatewayApi;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class DeleteStep : Step
{
    private List<string> _resources;

    public string Delete
    {
        set => _resources = value.Split(",").ToList();
    }

    public override ObjectEntity Execute(ObjectEntity state)
    {
        foreach (var res in _resources)
        {
            state.Delete(res);
        }

        return state;
    }
    
}