using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class IfStep : Step
{
    public enum ComparisonOperator
    {
        Equals, NotEqual, StartsWith, Contains, Exists
    }
    private ComparisonOperator _comparisonOperator;
    private string _comparand1;
    private string _comparand2;

    public string If
    {
        set
        {
            var parts = value.Split(" ", 3);
            if (parts.Length < 2)
            {
                throw new ApiConfigException("IfStep condition should at least 2 values (comparand1 operator comparand2)");
            }

            _comparand1 = parts[0];
            if (!Enum.TryParse(parts[1].Trim(), true, out _comparisonOperator))
            {
                throw new ApiRuntimeException("Invalid comparison operator in condition " + value);
            }

            if (_comparisonOperator != ComparisonOperator.Exists)
            {
                if (parts.Length != 2)
                {
                    throw new ApiConfigException("IfStep condition should have 3 values (comparand1 operator comparand2)");
                }
                _comparand2 = parts[2];
            }
        }
    }

    public List<Step>? Then { get; set; }
    public List<Step>? Else { get; set; }
    
    public override async Task<ObjectEntity> Execute(ObjectEntity state)
    {
        if (Evaluate(state))
        {
            if (Then != null)
            {
                state = await ApiOperation.Execute(Then, state);
            }
        }
        else
        {
            if (Else != null)
            {
                state = await ApiOperation.Execute(Else, state);
            }
        }

        return state;
    }

    private bool Evaluate(ObjectEntity state)
    {
        return _comparisonOperator switch
        {
            ComparisonOperator.Equals => CompareEquals(state),
            ComparisonOperator.NotEqual => !CompareEquals(state),
            ComparisonOperator.StartsWith => state.Substitute(_comparand1).StartsWith(state.Substitute(_comparand2)),
            ComparisonOperator.Contains => state.Substitute(_comparand1).Contains(state.Substitute(_comparand2)),
            ComparisonOperator.Exists => state.Find(_comparand1) != null,
            _ => throw new ArgumentOutOfRangeException("Invalid comparisonOperator " + _comparisonOperator)
        };
    }

    private bool CompareEquals(ObjectEntity state)
    {
        var find1 = state.Find(_comparand1);
        var find2 = state.Find(_comparand2);
        if (find1 != null && find1.Equals(find2)) return true;
        return state.Substitute(_comparand1).Equals(state.Substitute(_comparand2));
    }
}