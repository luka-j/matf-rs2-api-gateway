using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Processing;

public class RequestExecutor
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly ApiRepository _apis;
    private readonly EntityMapper _entityMapper;
    private readonly RequestResponseFilter _filter;
    private RequestProcessorGateway _requestProcessorGateway;

    public RequestExecutor(ApiRepository apis, EntityMapper entityMapper, RequestResponseFilter filter, RequestProcessorGateway requestProcessorGateway)
    {
        _apis = apis;
        _entityMapper = entityMapper;
        _filter = filter;
        _requestProcessorGateway = requestProcessorGateway;
    }

    public async ValueTask ExecuteRequest(string path, DateTime now, HttpContext httpContext)
    {
        var (specPath, apiConfig, operation) = ResolveOperation(path, now, httpContext.Request);
        var executionRequest = await MakeExecutionRequest(apiConfig, path, specPath, operation, 
            httpContext.Request, now);
        var executionResponse = await _requestProcessorGateway.ProcessRequest(executionRequest);
        await PopulateHttpResponse(executionResponse, operation, httpContext.Response);
    }

    private async ValueTask PopulateHttpResponse(ExecutionResponse response, OpenApiOperation spec, HttpResponse httpResponse)
    {
        if (!spec.Responses.TryGetValue(response.Status.ToString(), out var responseSpec))
        {
            throw new ApiRuntimeException("Undefined response status " + response.Status);
        }

        var filteredHeaders = _filter.FilterHeaders(responseSpec.Headers, response.Headers);
        if (filteredHeaders != null)
        {
            PopulateHttpHeaders(filteredHeaders, httpResponse.Headers);
        }

        httpResponse.Headers.ContentType = "application/json";
        
        httpResponse.StatusCode = response.Status;
        await httpResponse.StartAsync();
        
        var filteredResponse = _filter.FilterBody(responseSpec, response.ResponseBody);
        if (filteredResponse != null)
        {
            var parsedResponse = _entityMapper.MapToJsonNode(filteredResponse);
            var responseString = parsedResponse.ToJsonString();
            await httpResponse.Body.WriteAsync(Encoding.UTF8.GetBytes(responseString));
        }

        await httpResponse.CompleteAsync();
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
    }
    
    private async Task<ExecutionRequest> MakeExecutionRequest(ApiConfig config, string path, string specPath, 
        OpenApiOperation operation, HttpRequest httpRequest, DateTime now)
    {
        var headers = httpRequest.Headers;
        var query = httpRequest.Query;
        Entity? bodyEntity = null;
        if (httpRequest.ContentLength != null)
        {
            var body = await httpRequest.BodyReader.ReadAsync();
            if (body.IsCompleted)
            {
                bodyEntity = _entityMapper.MapToEntity(JsonNode.Parse(body.Buffer.ToString())!);
            }
            else
            {
                _logger.Error("Failed to read body, content: ${Body}", body);
                throw new ApiRuntimeException("Failed to read request body");
            }
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

        var (filteredPath, filteredQuery, filteredHeaders) = 
            _filter.FilterParams(operation.Parameters, pathEntity, headersEntity, queryEntity);
        var filteredBody = _filter.FilterBody(operation.RequestBody, bodyEntity);

        return new ExecutionRequest
        {
            ApiName = config.Id.Name,
            ApiVersion = config.Id.Version,
            Method = httpRequest.Method,
            Path = specPath,
            PathParameters = filteredPath,
            HeaderParameters = filteredHeaders,
            QueryParameters = filteredQuery,
            RequestBody = filteredBody,
            RequestMetadata = new RequestMetadata
            {
                StartTime = now.ToString(CultureInfo.InvariantCulture)
            }
        };
    }

    private Tuple<string, ApiConfig, OpenApiOperation> ResolveOperation(string path, DateTime now, HttpRequest request)
    {
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
            throw new PathNotFound("Could not find route for path " + path);
        }

        return new Tuple<string, ApiConfig, OpenApiOperation>(oasOperation.Value.Key, currentConfig,
            oasOperation.Value.Value);
    }

    private static PrimitiveObjectEntity ExtractPathParams(string realPath, string pathWithParams)
    {
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

        return pathParams;
    }
}