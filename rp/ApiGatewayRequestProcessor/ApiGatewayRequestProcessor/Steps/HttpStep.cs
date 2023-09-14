using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Utils;
using Serilog;

namespace ApiGatewayRequestProcessor.Steps;

public class HttpStep : Step
{
    private static readonly ApiGateway ApiGateway = new(); // not the prettiest solution (making ApiGateway not really a
                                                           // singleton), but also not quite harmful
    
    private string _api;
    private string _version;
    private string _resource;
    private string _operation;

    public string Http
    {
        set
        {
            var parts = value.Split(" ");
            if (parts.Length != 4)
            {
                throw new ApiConfigException(
                    "Http step expression is invalid, should be 'api version resource operation'");
            }

            (_api, _version, _resource, _operation) = (parts[0], parts[1], parts[2], parts[3]);
        }
    }
    
    public string? Body { get; set; }
    public string? Headers { get; set; }
    public string? QueryParams { get; set; }
    public string? PathParams { get; set; }
    public int Timeout { get; set; }
    public int Retries { get; set; }
    
    public string? Result { get; set; }


    public override async Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        var executionRequest = PackToExecutionRequest(state);
        var response = await ApiGateway.InvokeRequest(executionRequest);
        var responseEntity = ApiOperation.UnpackFromExecutionResponse(response);
        if (Result != null)
        {
            state.Insert(new Entity { Object = responseEntity }, Result);
        }

        return state;
    }

    private ExecutionRequest PackToExecutionRequest(ObjectEntity state)
    {
        var executionRequest = new ExecutionRequest
        {
            ApiName = _api,
            ApiVersion = _version,
            Method = _operation,
            Path = _resource,
            Timeout = Timeout,
            Repeat = Retries,
            RequestMetadata = new RequestMetadata
            {
                RequestId = state.Find("${request.id}")?.AsString(),
                IpAddress = state.Find("${request.ip}")?.AsString(),
                StartTime = state.Find("${request.startTime}")?.AsString()
            }
        };
        
        if (Body != null)
        {
            executionRequest.RequestBody = state.Find(Body);
            if (executionRequest.RequestBody == null)
            {
                Log.Warning("Cannot find body {Body} for HttpStep, not sending it", Body);
            }
        }

        if (Headers != null)
        {
            var headersEntity = state.Find(Headers);
            if (headersEntity is not { ContentCase: Entity.ContentOneofCase.Object })
            {
                Log.Warning("Headers entity for path {Headers} is missing or of wrong type, ignoring",
                    Headers);
            }
            else
            {
                executionRequest.HeaderParameters = headersEntity.Object.ConvertToPrimitiveOrListObjectEntity();
            }
        }
        
        if (QueryParams != null)
        {
            var queryEntity = state.Find(QueryParams);
            if (queryEntity is not { ContentCase: Entity.ContentOneofCase.Object })
            {
                Log.Warning("Query entity for path {Query} is missing or of wrong type, ignoring",
                    QueryParams);
            }
            else
            {
                executionRequest.QueryParameters = queryEntity.Object.ConvertToPrimitiveOrListObjectEntity();
            }
        }
        
        if (PathParams != null)
        {
            var pathEntity = state.Find(PathParams);
            if (pathEntity is not { ContentCase: Entity.ContentOneofCase.Object })
            {
                Log.Warning("Path entity for path {Path} is missing or of wrong type, ignoring",
                    PathParams);
            }
            else
            {
                executionRequest.PathParameters = pathEntity.Object.ConvertToPrimitiveObjectEntity();
            }
        }

        return executionRequest;
    }
}