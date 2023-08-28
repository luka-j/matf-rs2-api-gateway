using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Utils;
using CCO;
using Grpc.Core;

namespace ApiGatewayRequestProcessor.Steps;

public class CacheStep : Step
{
    private static readonly CcoGateway CcoGateway = CcoGateway.Instance;

    private enum CacheOperation
    {
        Read, Write
    }

    private CacheOperation _operation;
    public string Cache
    {
        set
        {
            if (!Enum.TryParse(value.ToUpper(), true, out _operation))
            {
                throw new ApiConfigException("Invalid cache operation: must be Read or Write");
            }
        }
    }
    
    public string Key { get; set; }
    public string Value { get; set; }
    public string Ttl { get; set; }

    private string _name, _version;
    public string Id
    {
        set
        {
            var parts= value.Split("/");
            if (parts.Length == 2)
            {
                _name = parts[0];
                _version = parts[1];
            }
            else if (parts.Length == 1)
            {
                _name = value;
                _version = "";
            }
            else
            {
                throw new ApiConfigException("Invalid cache id: " + value);
            }
        }
    }

    public string? Result { get; set; }
    
    public override async Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {        
        var configId = new ConfigIdentifier
        {
            ApiName = _name,
            ApiVersion = _version,
        };
        switch (_operation)
        {
            case CacheOperation.Read:
                var readResult = await CcoGateway.CacheRead(new CacheReadRequest
                {
                    Identifier = configId,
                    Key = state.Substitute(Key)
                });
                state.Insert(new Entity { String = readResult.Value }, Result);
                break;
            case CacheOperation.Write:
                var writeResult = await CcoGateway.CacheWrite(new CacheWriteRequest
                {
                    Identifier = configId,
                    Key = state.Substitute(Key),
                    Ttl = state.Substitute(Ttl),
                    Value = state.Substitute(Value)
                });
                if (Result is not null)
                {
                    state.Insert(new Entity { Boolean = writeResult.Success }, Result);
                }

                break;
        }

        return state;
    }
}