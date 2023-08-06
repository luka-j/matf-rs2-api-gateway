using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;

namespace ApiGatewayRequestProcessor.Steps;

public class ForeachStep : Step
{
    private string _element;
    private string _list;
    
    public string Foreach { 
        set
        {
            var parts = value.Split(":");
            if (parts.Length != 2)
            {
                throw new ApiConfigException("Invalid foreach syntax, should contain single colon");
            }

            _element = parts[0].Trim();
            _list = parts[1].Trim();
        }
    }
    
    public List<Step> Do { get; set; }
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        return state;
    }
}