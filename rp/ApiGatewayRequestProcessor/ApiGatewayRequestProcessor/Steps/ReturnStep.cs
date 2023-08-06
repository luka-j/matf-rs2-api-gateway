using ApiGatewayApi;

namespace ApiGatewayRequestProcessor.Steps;

public class ReturnStep : Step
{
    public string Abort
    {
        set => _status = value;
    }

    private string _status;
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        return state;
    }
}