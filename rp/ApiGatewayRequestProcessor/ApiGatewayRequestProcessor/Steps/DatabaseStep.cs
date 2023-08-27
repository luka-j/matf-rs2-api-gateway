using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Utils;
using CCO;

namespace ApiGatewayRequestProcessor.Steps;

public class DatabaseStep : Step
{
    private static readonly CcoGateway CcoGateway = CcoGateway.Instance;

    private enum DatabaseOperation
    {
        Read, Write, Delete
    }

    private DatabaseOperation _operation;
    public string Database
    {
        set
        {
            if (!Enum.TryParse(value.ToUpper(), true, out _operation))
            {
                throw new ApiConfigException("Invalid database operation: must be READ, WRITE or DELETE");
            }
        }
    }
    
    public string Content { get; set; }

    private string _name, _version;
    public string Id
    {
        set
        {
            var parts= value.Split("/");
            if (parts.Length != 2)
            {
                throw new ApiConfigException("Database id must be in form 'name/version'");
            }

            _name = parts[0];
            _version = parts[0];
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
            case DatabaseOperation.Read:
                var readResult = await CcoGateway.DatabaseRead(new DatabaseReadRequest
                {
                    Identifier = configId
                });
                state.Insert(Convert(readResult.Items), Result);
                break;
            case DatabaseOperation.Write:
                var amount = state.Substitute(Content);
                if (!int.TryParse(amount, out var amountInt))
                {
                    throw new ApiRuntimeException("Only inserting ints is supported!");
                }
                var writeResult = await CcoGateway.DatabaseWrite(new DatabaseWriteRequest
                {
                    Amount = amountInt,
                    Identifier = configId
                });
                if (Result is not null)
                {
                    state.Insert(new Entity { Boolean = writeResult.Success}, Result);
                }

                break;
            case DatabaseOperation.Delete:
                var deleteId = state.Substitute(Content);
                var deleteResult = await CcoGateway.DatabaseDelete(new DatabaseDeleteRequest
                {
                    Identifier = configId,
                    Id = deleteId
                });
                if (Result is not null)
                {
                    state.Insert(new Entity { Boolean = deleteResult.Success}, Result);
                }
                break;
        }

        return state;
    }

    private Entity Convert(IEnumerable<DatabaseItem> items)
    {
        var mapped = items.Select(it => new Entity
        {
            Object = new ObjectEntity
            {
                Properties =
                {
                    {"id", new Entity { String = it.Id }},
                    {"amount", new Entity {Integer = it.Amount }}
                }
            }
        });
        return new Entity
        {
            List = new ListEntity
            {
                Value = { mapped }
            }
        };
    }
}