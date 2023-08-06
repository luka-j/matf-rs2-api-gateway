using ApiGatewayApi;

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
        return state;
    }
    
}