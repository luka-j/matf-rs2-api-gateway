using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;

namespace ApiGatewayRequestProcessor.Steps;

public class IfStep : Step
{
    private string _comparisonOperator;
    private string _comparand1;
    private string _comparand2;

    public string If
    {
        set
        {
            var parts = value.Split(" ");
            if (parts.Length != 3)
            {
                throw new ApiConfigException("IfStep condition should have 3 values (comparand1 operator comparand2)");
            }

            _comparand1 = parts[0];
            _comparisonOperator = parts[1];
            _comparand2 = parts[2];
        }
    }

    public List<Step> Then { get; set; }
    public List<Step> Else { get; set; }
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        return state;
    }
}