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
        var obj = new ObjectEntity
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
            }
        };
        if (request.RequestBody != null)
        {
            obj.Properties.Add("body", request.RequestBody);
        }
        if (request.HeaderParameters != null)
        {
            obj.Properties.Add("headers", request.HeaderParameters.ConvertToObject());
        }
        if (request.QueryParameters != null)
        {
            obj.Properties.Add("query", request.QueryParameters.ConvertToObject());
        }
        if (request.PathParameters != null)
        {
            obj.Properties.Add("path", request.PathParameters.ConvertToObject());
        }

        return obj;
    }

    private static ExecutionResponse UnpackToExecutionResponse(ObjectEntity entity)
    {
        var response = new ExecutionResponse();
        var status = 200;
        if (entity.Properties.TryGetValue("status", out var statusEntity))
        {
            if (statusEntity.ContentCase == Entity.ContentOneofCase.Integer)
            {
                status = (int) statusEntity.Integer;
            }
            else if (statusEntity.ContentCase == Entity.ContentOneofCase.String)
            {
                if (!int.TryParse(statusEntity.String, out status))
                {
                    throw new ApiRuntimeException("Invalid value for status " + statusEntity.String);
                }
            }
            else if (statusEntity.ContentCase == Entity.ContentOneofCase.Boolean)
            {
                status = statusEntity.Boolean ? 200 : 500;
            }
            else
            {
                throw new ApiRuntimeException("Invalid type for status " + statusEntity.ContentCase);
            }
        }

        response.Status = status;

        if (entity.Properties.TryGetValue("headers", out var headersEntity))
        {
            if (headersEntity.ContentCase != Entity.ContentOneofCase.Object)
            {
                throw new ApiRuntimeException("Response headers field is " + headersEntity.ContentCase +
                                              ", expected object!");
            }

            response.Headers = headersEntity.Object.ConvertToPrimitiveOrListObjectEntity();
        }

        if (entity.Properties.TryGetValue("body", out var bodyEntity))
        {
            response.ResponseBody = bodyEntity;
        }

        return response;
    }

    private static bool IsFinalState(ObjectEntity entity)
    {
        var finalStateMark = entity.Find(FinalStateMarkLocation);
        return finalStateMark != null;
    }

}