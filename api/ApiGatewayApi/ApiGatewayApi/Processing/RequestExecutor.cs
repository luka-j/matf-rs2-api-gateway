using System.IO.Pipelines;
using System.Text;
using System.Text.Json.Nodes;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Controllers;
using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Services;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Processing;

public class RequestExecutor
{
    private static readonly Serilog.ILogger Logger = Serilog.Log.Logger;
    
    private readonly ApiRepository _apis;
    private readonly EntityMapper _entityMapper;
    private readonly RequestResponseFilter _filter;
    private readonly RequestProcessorGateway _requestProcessorGateway;
    private readonly ControllerUtils _controllerUtils;
    private readonly MetricsService _metrics;

    public RequestExecutor(ApiRepository apis, EntityMapper entityMapper, RequestResponseFilter filter, 
        RequestProcessorGateway requestProcessorGateway, ControllerUtils controllerUtils, MetricsService metrics)
    {
        _apis = apis;
        _entityMapper = entityMapper;
        _filter = filter;
        _requestProcessorGateway = requestProcessorGateway;
        _controllerUtils = controllerUtils;
        _metrics = metrics;
    }

    public async ValueTask ExecuteRequest(string path, DateTime now, RequestMetadata requestMetadata, HttpContext httpContext)
    {
        var (specPath, apiConfig, operation) = ResolveOperation(path, now, httpContext.Request);
        _controllerUtils.AddCorsHeadersToResponse(httpContext, apiConfig);
        if (operation == null && httpContext.Request.Method.ToUpper() == "OPTIONS") // if this was a CORS preflight
        {
            await httpContext.Response.CompleteAsync();
            _metrics.RecordApiCorsRequestTime(apiConfig, specPath, httpContext.Request.Method.ToUpper(), DateTime.Now - now);
            return;
        }
        var executionRequest = await MakeExecutionRequest(apiConfig, path, specPath, operation, 
            httpContext.Request, requestMetadata);
        var executionResponse = await _requestProcessorGateway.ProcessRequest(executionRequest);
        await PopulateHttpResponse(executionResponse, operation, httpContext.Response);
        _metrics.RecordApiRequestTime(MetricsService.ApiRequestTime, executionRequest, executionResponse, DateTime.Now - now);
    }

    private async ValueTask PopulateHttpResponse(ExecutionResponse response, OpenApiOperation spec, HttpResponse httpResponse)
    {
        Logger.Debug("Populating HttpResponse from ExecutionResponse {ExecutionResponse}", response);
        if (!spec.Responses.TryGetValue(response.Status.ToString(), out var responseSpec))
        {
            throw new ApiRuntimeException("Undefined response status " + response.Status);
        }

        var filteredHeaders = _filter.FilterHeaders(responseSpec.Headers, response.Headers);
        if (filteredHeaders != null)
        {
            Logger.Debug("Populating headers from entity {FilteredHeaders}", filteredHeaders);
            PopulateHttpHeaders(filteredHeaders, httpResponse.Headers);
        }

        httpResponse.Headers.ContentType = "application/json";
        httpResponse.StatusCode = response.Status;
        
        var filteredResponse = _filter.FilterBody(responseSpec, response.ResponseBody);
        string? responseString = null;
        if (filteredResponse != null)
        {
            var parsedResponse = _entityMapper.MapToJsonNode(filteredResponse);
            responseString = parsedResponse.ToJsonString();
            httpResponse.ContentLength = responseString.Length;
        }
        
        Logger.Debug("Sending response (body: {Body}) to caller", responseString);

        await httpResponse.StartAsync();
        if (responseString != null)
        {
            await httpResponse.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(responseString));
        }

        await httpResponse.CompleteAsync();
        Logger.Debug("Finished sending off http response");
    }

    private void PopulateHttpHeaders(PrimitiveOrListObjectEntity data, IHeaderDictionary headers)
    {
        foreach (var (key, value) in data.Properties)
        {
            switch (value.ContentCase)
            {
                case PrimitiveOrList.ContentOneofCase.Primitive:
                    headers.Add(key, _entityMapper.PrimitiveEntityToString(value.Primitive));
                    break;
                case PrimitiveOrList.ContentOneofCase.List:
                    headers.Add(key, new StringValues(
                        value.List.Value.Select(_entityMapper.PrimitiveEntityToString).ToArray()));
                    break;
            }
        }
        Logger.Debug("Finished populating headers, result is {Headers}", headers);
    }
    
    private async Task<ExecutionRequest> MakeExecutionRequest(ApiConfig config, string path, string specPath, 
        OpenApiOperation operation, HttpRequest httpRequest, RequestMetadata requestMetadata)
    {
        Logger.Debug("Making ExecutionRequest for {Path} (spec: {SpecPath}), resolved operation {@Operation}",
            path, specPath, operation);
        var headers = httpRequest.Headers;
        var query = httpRequest.Query;
        Entity? bodyEntity = null;
        if (httpRequest.ContentLength is > 0)
        {
            var bodyString = await ReadBody(httpRequest);
            bodyEntity = _entityMapper.MapToEntity(JsonNode.Parse(bodyString)!);
        }

        var headersEntity = new PrimitiveOrListObjectEntity();
        _entityMapper.MapToPrimitiveOrListObjectEntity(headers
            .Select(header => new KeyValuePair<string,IEnumerable<string>>
                (header.Key, header.Value.ToList()!)), headersEntity);
        var queryEntity = new PrimitiveOrListObjectEntity();
        _entityMapper.MapToPrimitiveOrListObjectEntity(query
            .Select(q => new KeyValuePair<string,IEnumerable<string>>
                (q.Key, q.Value.ToList()!)), queryEntity);
        var pathEntity = ExtractPathParams(path, specPath);

        var (filteredPath, filteredHeaders, filteredQuery) = 
            _filter.FilterParams(operation.Parameters, pathEntity, headersEntity, queryEntity);
        var filteredBody = _filter.FilterBody(operation.RequestBody, bodyEntity);
        
        var ret = new ExecutionRequest
        {
            ApiName = config.Id.Name,
            ApiVersion = config.Id.Version,
            Method = httpRequest.Method,
            Path = specPath,
            PathParameters = filteredPath,
            HeaderParameters = filteredHeaders,
            QueryParameters = filteredQuery,
            RequestBody = filteredBody,
            RequestMetadata = requestMetadata,
        };
        Logger.Debug("Made execution request: {ExecutionRequest}", ret);
        return ret;
    }

    private async Task<string> ReadBody(HttpRequest request)
    {
        var body = new StringBuilder();
        ReadResult readResult;
        do
        {
            readResult = await request.BodyReader.ReadAsync();
            body.Append(Encoding.UTF8.GetString(readResult.Buffer));
            request.BodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
        } while (readResult is { IsCompleted: false, IsCanceled: false });

        await request.BodyReader.CompleteAsync();

        var bodyString = body.ToString();
        Logger.Debug("Read request body: {BodyString}", bodyString);
        return bodyString;
    }

    private Tuple<string, ApiConfig, OpenApiOperation?> ResolveOperation(string path, DateTime now, HttpRequest request)
    {
        Logger.Debug("Resolving operation for path {Path}, now is {Now}", path, now);
        var pathSegments = path.Split('/', 3);
        if (pathSegments.Length < 3)
        {
            throw new PathNotFound("API not found");
        }

        var apiName = pathSegments[0];
        var apiVersion = pathSegments[1];
        var currentConfig = _apis.Frontends.GetCurrentConfig(new ApiIdentifier(apiName, apiVersion), now);
        if (currentConfig == null)
        {
            throw new PathNotFound("API " + apiName + "/" + apiVersion + " not found");
        }

        var oasOperation = currentConfig.ResolveOperation(request.Method, pathSegments[2]);
        if (!oasOperation.HasValue)
        {
            if (request.Method.ToUpper() != "OPTIONS") throw new PathNotFound("Could not find route for path " + path);
            
            // OPTIONS might be a preflight POST request, so we still need to resolve at least the path item
            var pathItem = currentConfig.ResolvePath(pathSegments[2]);
            if (pathItem == null)
            {
                throw new PathNotFound("Could not find route for path " + path);
            }

            return new Tuple<string, ApiConfig, OpenApiOperation?>(pathItem.SpecPath, currentConfig, null);
        }

        Logger.Debug("Resolved operation {Operation} from config {ConfigId}", oasOperation.Value, 
            currentConfig.Id);
        return new Tuple<string, ApiConfig, OpenApiOperation?>(oasOperation.Value.Key, currentConfig,
            oasOperation.Value.Value);
    }

    private static PrimitiveObjectEntity ExtractPathParams(string realPath, string pathWithParams)
    {
        Logger.Debug("Extracting path params from {PathWithParams} (template: {SpecPath})",
            realPath, pathWithParams);
        var trimmedPathSegments = realPath.Split('/')[2..];
        if (pathWithParams.StartsWith('/'))
        {
            pathWithParams = pathWithParams[1..];
        }
        var parametrizedPathSegments = pathWithParams.Split('/');
        if (trimmedPathSegments.Length != parametrizedPathSegments.Length)
        {
            throw new ApiRuntimeException("Real path contains " + trimmedPathSegments.Length + " segments ("
                                          + trimmedPathSegments + "), but spec path contains " +
                                          parametrizedPathSegments.Length + "(" + pathWithParams + ")");
        }

        var pathParams = new PrimitiveObjectEntity();
        for (var i = 0; i < trimmedPathSegments.Length; i++)
        {
            if (parametrizedPathSegments[i].StartsWith('{') && parametrizedPathSegments[i].EndsWith('}'))
            {
                var paramName = parametrizedPathSegments[i][1..(parametrizedPathSegments[i].Length - 1)];
                pathParams.Properties.Add(paramName, new PrimitiveEntity {String = trimmedPathSegments[i]});
            }
        }

        Logger.Debug("Extracted path params: {PathParams}", pathParams);
        return pathParams;
    }
}