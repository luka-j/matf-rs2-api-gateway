using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class ReturnStep : Step
{
    public string Return
    {
        set
        {
            var parts = value.Split(" ");
            if (parts.Length > 2)
            {
                throw new ApiConfigException("Return step can contain at most 2 parts!");
            }
            _status = parts[0].Trim();
            if (parts.Length == 2)
            {
                _result = parts[1].Trim();
            }
        }
    }

    private string _status;
    private string? _result;
    
    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        if (_result != null)
        {
            var result = state.Find(_result);
            if (result == null)
            {
                throw new ApiConfigException("Cannot find result entity " + _result);
            }

            if (result.ContentCase != Entity.ContentOneofCase.Object)
            {
                throw new ApiConfigException("Result entity is not an object, is " + result.ContentCase);
            }

            state = result.Object;
        }

        state.Insert(new Entity { String = state.Substitute(_status) }, ApiOperation.StatusLocation);
        state.Insert(new Entity { Boolean = true }, ApiOperation.FinalStateMarkLocation);
        return Task.FromResult(state);
    }
}