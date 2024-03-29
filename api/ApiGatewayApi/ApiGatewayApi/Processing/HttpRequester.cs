using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Processing;


public class HttpRequester
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly ApiRepository _apiRepository;
    private readonly RequestResponseFilter _filter;
    private readonly EntityMapper _entityMapper;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly MetricsService _metrics;

    public HttpRequester(RequestResponseFilter filter, EntityMapper entityMapper, 
        ApiRepository apiRepository, IHttpClientFactory httpClientFactory, MetricsService metrics)
    {
        _filter = filter;
        _entityMapper = entityMapper;
        _apiRepository = apiRepository;
        _httpClientFactory = httpClientFactory;
        _metrics = metrics;
    }
    
    public async Task<ExecutionResponse> MakeRequest(ExecutionRequest request)
    {
        var requestStart = DateTime.Now;
        var apiConfig = _apiRepository.Backends.GetCurrentConfig(
            new ApiIdentifier(request.ApiName, request.ApiVersion), 
            DateTime.Parse(request.RequestMetadata.StartTime, null, DateTimeStyles.RoundtripKind));
        if (apiConfig == null)
        {
            throw new PathNotFound("API not found");
        }

        if (!Enum.TryParse(request.Method, true, out OperationType operationType))
        {
            throw new BadRequestException("Invalid method");
        }

        var operation = apiConfig.Spec.Paths[request.Path]?.Operations[operationType];
        if (operation == null)
        {
            throw new PathNotFound("Path not found");
        }
        _logger.Debug("Resolved OAS operation {Operation}", operation.OperationId);

        var requestBodyEntity = _filter.FilterBody(operation.RequestBody, request.RequestBody);
        var (pathParams, headerParams, queryParams) = 
            _filter.FilterParams(operation.Parameters, request.PathParameters, request.HeaderParameters, 
                request.QueryParameters);

        var httpClient = _httpClientFactory.CreateClient();
        var httpMessage = MakeHttpRequestMessage(request.Method,
            ConcatPaths(apiConfig.Spec.Servers[0].Url, request.Path), requestBodyEntity, 
            pathParams, headerParams, queryParams);
        _logger.Debug("Sending HTTP message: {HttpMessage}", httpMessage);
        
        var response = await httpClient.SendAsync(httpMessage);
        _logger.Debug("Got HTTP response: {Response}", response);
        var responseSpec = ResolveOpenApiResponse(operation.Responses, response);

        var responseHeaders = ParseHeaders(response.Headers, response.Content.Headers);
        var executionResponse = new ExecutionResponse
        {
            Status = (int) response.StatusCode,
            Headers = _filter.FilterHeaders(responseSpec.Headers, responseHeaders),
        };
        var responseBody = JsonNode.Parse(await response.Content.ReadAsStreamAsync());
        _logger.Debug("Got HTTP response body: {ResponseBody}", responseBody);
        if (responseBody != null)
        {
            var mappedBody = _entityMapper.MapToEntity(responseBody);
            var filteredBody = _filter.FilterBody(responseSpec, mappedBody);
            executionResponse.ResponseBody = filteredBody;
        }
        else
        {
            _filter.FilterBody(responseSpec, null);
        }

        _logger.Debug("Returning execution response: {ExecutionResponse}", executionResponse);
        _metrics.RecordApiRequestTime(MetricsService.BackendRequestTime, request, executionResponse, DateTime.Now - requestStart);
        return executionResponse;
    }

    public HttpRequestMessage MakeHttpRequestMessage(string method, string path, Entity? requestBody,
        PrimitiveObjectEntity? pathParams, PrimitiveOrListObjectEntity? headers, PrimitiveOrListObjectEntity? queryParams)
    {
        var message = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = BuildUri(path, pathParams, queryParams)
        };
        PopulateContent(message, requestBody, headers);
        return message;
    }

    private Uri BuildUri(string path, PrimitiveObjectEntity? pathParams, PrimitiveOrListObjectEntity? queryParams)
    {
        var parsedPath = ReplacePathParams(path, pathParams);
        if (queryParams != null)
        {
            parsedPath = QueryHelpers.AddQueryString(parsedPath, GetQueryParams(queryParams));
        }
        return new Uri(parsedPath);
    }

    private void PopulateContent(HttpRequestMessage message, Entity? body, PrimitiveOrListObjectEntity? headers)
    {
        if (body != null && body.ContentCase != Entity.ContentOneofCase.None)
        {
            var requestBody = _entityMapper.MapToJsonNode(body);
            var content = JsonContent.Create(requestBody);
            message.Content = content;
        }

        if (headers != null)
        {
            PopulateHeaders(headers, message.Headers);
        }
    }

    private void PopulateHeaders(PrimitiveOrListObjectEntity headerParams, HttpRequestHeaders headers)
    {
        foreach (var (key, value) in headerParams.Properties)
        {
            if (value.ContentCase == PrimitiveOrList.ContentOneofCase.Primitive)
            {
                headers.Add(key, _entityMapper.PrimitiveEntityToString(value.Primitive));
            } else if (value.ContentCase == PrimitiveOrList.ContentOneofCase.List)
            {
                var headerValues = value.List.Value.Select(_entityMapper.PrimitiveEntityToString);
                headers.Add(key, headerValues);
            }
        }
    }

    private string ReplacePathParams(string path, PrimitiveObjectEntity? pathParams)
    {
        if (pathParams == null) return path;
        
        var segments = path.Split('/');
        var sb = new StringBuilder();
        foreach (var segment in segments)
        {
            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                var paramName = segment[1..^1];
                if (pathParams.Properties.TryGetValue(paramName, out var property))
                {
                    sb.Append(_entityMapper.PrimitiveEntityToString(property));
                }
                else
                {
                    throw new BadRequestException("Missing path param: " + paramName);
                }
            }
            else
            {
                sb.Append(segment);
            }

            sb.Append('/');
        }

        if (!path.EndsWith('/'))
        {
            sb.Remove(sb.Length - 1, 1);
        }

        return sb.ToString();
    }

    private IEnumerable<KeyValuePair<string, StringValues>> GetQueryParams(PrimitiveOrListObjectEntity queryParams)
    {
        var ret = new List<KeyValuePair<string, StringValues>>();
        foreach (var queryParam in queryParams.Properties)
        {
            switch (queryParam.Value.ContentCase)
            {
                case PrimitiveOrList.ContentOneofCase.List:
                    var vals = queryParam.Value.List.Value.Select(_entityMapper.PrimitiveEntityToString);
                    ret.Add(new KeyValuePair<string, StringValues>(queryParam.Key, 
                        new StringValues(vals.ToArray())));
                    break;
                case PrimitiveOrList.ContentOneofCase.Primitive:
                    var val = _entityMapper.PrimitiveEntityToString(queryParam.Value.Primitive);
                    ret.Add(new KeyValuePair<string, StringValues>(queryParam.Key, new StringValues(val)));
                    break;
            }
        }

        return ret;
    }

    private PrimitiveOrListObjectEntity ParseHeaders(HttpResponseHeaders responseHeaders, 
        HttpContentHeaders contentHeaders)
    {
        var result = new PrimitiveOrListObjectEntity();
        _entityMapper.MapToPrimitiveOrListObjectEntity(responseHeaders, result);
        _entityMapper.MapToPrimitiveOrListObjectEntity(contentHeaders, result);
        return result;
    }

    private static OpenApiResponse ResolveOpenApiResponse(OpenApiResponses responses, HttpResponseMessage message)
    {
        var stringCode = ((int)message.StatusCode).ToString();
        
        if (responses.TryGetValue(stringCode, out var response)) return response;
        throw new ApiRuntimeException("Invalid response " + stringCode);
    }

    private static string ConcatPaths(string p1, string p2)
    {
        var slashes = 0;
        slashes += p1.EndsWith('/') ? 1 : 0;
        slashes += p2.StartsWith('/') ? 1 : 0;
        return slashes switch
        {
            0 => p1 + '/' + p2,
            1 => p1 + p2,
            _ => p1 + p2[1..]
        };
    }
}