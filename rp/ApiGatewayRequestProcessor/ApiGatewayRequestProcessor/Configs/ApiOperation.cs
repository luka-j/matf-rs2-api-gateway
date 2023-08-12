using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Steps;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Configs;

public class ApiOperation
{
    public string? Prepare { get; set; }
    public List<Step>? Steps { get; set; }
    public bool? Pass { get; set; }
    public string? Finalize { get; set; }

    public const string FinalStateMarkLocation = "${__internal.finished}";
    public const string BreakMarkLocation = "${__internal.break}";
    
    public const string StatusLocation = "${status}";

    public async Task<ExecutionResponse> Execute(ExecutionRequest request, ApiGateway gateway, 
        Dictionary<string, List<Step>> stepRepository)
    {
        ExecutionResponse? response = null;
        var state = PackExecutionRequest(request);
        if (Prepare != null)
        {
            if (!stepRepository.TryGetValue(Prepare, out var prepareSteps))
            {
                throw new ApiRuntimeException("Cannot find Prepare steps " + Prepare);
            }
            state = await ExecuteSteps(prepareSteps, state, stepRepository);
            if (IsFinalState(state))
            {
                return UnpackToExecutionResponse(state);
            }
        }

        if (Pass == true)
        {
            response = await gateway.InvokeRequest(request);
        }
        else if(Steps != null)
        {
            state = await ExecuteSteps(Steps, state, stepRepository);
            if (IsFinalState(state))
            {
                return UnpackToExecutionResponse(state);
            }
        }

        if (Finalize != null)
        {
            if (!stepRepository.TryGetValue(Finalize, out var finalizeSteps))
            {
                throw new ApiRuntimeException("Cannot find Prepare steps " + Prepare);
            }
            if (response != null)
            {
                state.Insert(new Entity { Object = UnpackFromExecutionResponse(response) }, "response");
            }
            state = await ExecuteSteps(finalizeSteps, state, stepRepository);
        }

        if (Pass == true && Steps == null && Finalize == null)
        {
            return response!;
        }
        else
        {
            return UnpackToExecutionResponse(state);
        }
    }

    public static async Task<ObjectEntity> ExecuteSteps(List<Step> steps, ObjectEntity state,
        Dictionary<string, List<Step>>? stepRepository)
    {
        foreach (var step in steps)
        {
            state = await step.Execute(state, stepRepository);
            if (IsBreakState(state))
            {
                state.Delete(BreakMarkLocation);
                break;
            }
            
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
    
    public static ObjectEntity UnpackFromExecutionResponse(ExecutionResponse response)
    {
        return new ObjectEntity
        {
            Properties =
            {
                { "body", response.ResponseBody },
                { "headers", response.Headers.ConvertToObject() },
                { "status", new Entity { Integer = response.Status } }
            }
        };
    }

    private static bool IsFinalState(ObjectEntity entity)
    {
        var finalStateMark = entity.Find(FinalStateMarkLocation);
        return finalStateMark != null;
    }

    private static bool IsBreakState(ObjectEntity entity)
    {
        var breakStateMark = entity.Find(BreakMarkLocation);
        return breakStateMark != null;
    }

}