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

    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        foreach (var res in _resources)
        {
            state.Delete(res);
        }

        return Task.FromResult(state);
    }
    
}