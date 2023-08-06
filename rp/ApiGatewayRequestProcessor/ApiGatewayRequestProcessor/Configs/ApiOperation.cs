using ApiGatewayApi;
using ApiGatewayRequestProcessor.Steps;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiOperation
{
    public List<Step> Steps { get; set; }
    public bool Pass { get; set; }

    public ExecutionResponse Execute(ExecutionRequest request)
    {
        return new ExecutionResponse();
    }
}