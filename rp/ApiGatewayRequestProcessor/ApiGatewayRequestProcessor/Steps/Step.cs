using ApiGatewayApi;

namespace ApiGatewayRequestProcessor.Steps;

public abstract class Step
{
    public abstract Task<ObjectEntity> Execute(ObjectEntity state);
}