using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;

namespace ApiGatewayRequestProcessor.Steps;

public class InvokeStep : Step
{
    public string Invoke { get; set; }

    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        if (stepRepository == null)
        {
            throw new ApiRuntimeException("Attempted to invoke step " + Invoke + ", but stepRepository is null");
        }

        if (!stepRepository.ContainsKey(Invoke))
        {
            throw new ApiRuntimeException("Attempted to invoke step " + Invoke + ", but it isn't defined");
        }

        return ApiOperation.ExecuteSteps(stepRepository[Invoke], state, stepRepository);
    }
}