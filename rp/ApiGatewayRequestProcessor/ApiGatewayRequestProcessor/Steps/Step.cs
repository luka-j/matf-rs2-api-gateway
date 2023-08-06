using ApiGatewayApi;

namespace ApiGatewayRequestProcessor.Steps;

public abstract class Step
{
    public string? Result { get; set; }

    public abstract ObjectEntity Execute(ObjectEntity state);
}