using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiOperation
{
    public List<Step>? Steps { get; set; }
    public bool? Pass { get; set; }

    public const string FinalStateMarkLocation = "${__internal.finished}";

    public async Task<ExecutionResponse> Execute(ExecutionRequest request, ApiGateway gateway)
    {
        if (Pass == true)
        {
            return await gateway.InvokeRequest(request);
        }

        if (Steps == null || Steps.Count == 0)
        {
            throw new ApiRuntimeException("ApiOperation has undefined steps!");
        }

        var state = PackExecutionRequest(request);
        state = await ExecuteSteps(Steps, state);

        return UnpackToExecutionResponse(state);
    }

    public static async Task<ObjectEntity> ExecuteSteps(List<Step> steps, ObjectEntity state)
    {
        foreach (var step in steps)
        {
            state = await step.Execute(state);
            if (IsFinalState(state)) break;
        }

        return state;
    }

    private static ObjectEntity PackExecutionRequest(ExecutionRequest request)
    {
        return new ObjectEntity
        {
            Properties =
            {
                {
                    "endpoint", new Entity
                    {
                        Object = new ObjectEntity
                        {
                            Properties =
                            {
                                { "api", new Entity { String = request.ApiName } },
                                { "version", new Entity { String = request.ApiVersion } },
                                { "path", new Entity { String = request.Path } },
                                { "method", new Entity { String = request.Method } }
                            }
                        }
                    }
                },
                {
                    "request", new Entity
                    {
                        Object = new ObjectEntity
                        {
                            Properties =
                            {
                                { "id", new Entity { String = request.RequestMetadata.RequestId } },
                                { "ip", new Entity { String = request.RequestMetadata.IpAddress } },
                                { "startTime", new Entity { String = request.RequestMetadata.StartTime } }
                            }
                        }
                    }
                },
                { "body", request.RequestBody },
                { "headers", request.HeaderParameters.ConvertToObject() },
                { "query", request.QueryParameters.ConvertToObject() },
                { "path", request.PathParameters.ConvertToObject() }
            }
        };
    }

    private static ExecutionResponse UnpackToExecutionResponse(ObjectEntity entity)
    {
        int status = 200;
        if (entity.Properties.TryGetValue("status", out var statusEntity))
        {
            if (statusEntity.ContentCase != Entity.ContentOneofCase.Integer)
            {
                throw new ApiRuntimeException("Invalid type for status " + statusEntity.ContentCase);
            }

            status = (int) statusEntity.Integer;
        }

        PrimitiveOrListObjectEntity? headers = null;
        if (entity.Properties.TryGetValue("headers", out var headersEntity))
        {
            if (headersEntity.ContentCase != Entity.ContentOneofCase.Object)
            {
                throw new ApiRuntimeException("Response headers field is " + headersEntity.ContentCase +
                                              ", expected object!");
            }

            headers = headersEntity.Object.ConvertToPrimitiveOrListObjectEntity();
        }

        return new ExecutionResponse
        {
            Status = status,
            ResponseBody = entity.Properties["body"],
            Headers = headers
        };
    }

    private static bool IsFinalState(ObjectEntity entity)
    {
        var finalStateMark = entity.Find(FinalStateMarkLocation);
        return finalStateMark != null;
    }

}