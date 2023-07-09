using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
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

    public HttpRequester(RequestResponseFilter filter, EntityMapper entityMapper, 
        ApiRepository apiRepository, IHttpClientFactory httpClientFactory)
    {
        _filter = filter;
        _entityMapper = entityMapper;
        _apiRepository = apiRepository;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<ExecutionResponse> MakeRequest(ExecutionRequest request)
    {
        var apiConfig = _apiRepository.Backends.GetCurrentConfig(
            new ApiIdentifier(request.ApiName, request.ApiVersion), 
            DateTime.Parse(request.RequestMetadata.StartTime));
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
        httpClient.BaseAddress = new Uri(apiConfig.Spec.Servers[0].Url);
        var httpMessage = MakeHttpRequestMessage(request.Method, apiConfig.Spec.Servers[0].Url + request.Path, requestBodyEntity, pathParams,
            headerParams, queryParams);
        _logger.Debug("Sending HTTP message: {HttpMessage}", httpMessage);
        
        var response = await httpClient.SendAsync(httpMessage);
        _logger.Debug("Got HTTP response: {Response}", response);
        var responseSpec = ResolveOpenApiResponse(operation.Responses, response);

        var executionResponse = new ExecutionResponse();
        executionResponse.Status = (int) response.StatusCode;
        var responseHeaders = ParseHeaders(response.Headers, response.Content.Headers);
        executionResponse.Headers = _filter.FilterHeaders(responseSpec.Headers, responseHeaders);
        var responseBody = JsonNode.Parse(await response.Content.ReadAsStreamAsync());
        _logger.Debug("Got HTTP response body: {ResponseBody}", responseBody);
        if (responseBody != null)
        {
            var mappedBody = _entityMapper.MapToEntity(responseBody);
            var filteredBody = _filter.FilterBody(responseSpec.Content["application/json"], mappedBody);
            executionResponse.ResponseBody = filteredBody;
        }

        _logger.Debug("Returning execution response: {ExecutionResponse}", executionResponse);
        return executionResponse;
    }

    private HttpRequestMessage MakeHttpRequestMessage(string method, string path, Entity requestBody,
        PrimitiveObjectEntity? pathParams, PrimitiveObjectEntity? headers, PrimitiveOrListObjectEntity? queryParams)
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

    private void PopulateContent(HttpRequestMessage message, Entity? body, PrimitiveObjectEntity? headers)
    {
        if (body != null)
        {
            var requestBody = _entityMapper.MapToJsonNode(body);
            var content = new StringContent(requestBody.ToJsonString());
            message.Content = content;
            message.Headers.Add("Content-Type", "application/json");
        }

        if (headers != null)
        {
            PopulateHeaders(headers, message.Headers);
        }
    }

    private static void PopulateHeaders(PrimitiveObjectEntity headerParams, HttpRequestHeaders headers)
    {
        foreach (var (key, value) in headerParams.Properties)
        {
            headers.Add(key, PrimitiveEntityToString(value));
        }
    }

    private static string ReplacePathParams(string path, PrimitiveObjectEntity? pathParams)
    {
        if (pathParams == null) return path;
        
        var segments = path.Split('/');
        var sb = new StringBuilder();
        foreach (var segment in segments)
        {
            if (segment.StartsWith(":"))
            {
                var paramName = segment[1..];
                if (pathParams.Properties.TryGetValue(paramName, out var property))
                {
                    sb.Append(property);
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
                    var vals = queryParam.Value.List.Value.Select(PrimitiveEntityToString);
                    ret.Add(new KeyValuePair<string, StringValues>(queryParam.Key, 
                        new StringValues(vals.ToArray())));
                    break;
                case PrimitiveOrList.ContentOneofCase.Primitive:
                    var val = PrimitiveEntityToString(queryParam.Value.Primitive);
                    ret.Add(new KeyValuePair<string, StringValues>(queryParam.Key, new StringValues(val)));
                    break;
            }
        }

        return ret;
    }

    private static PrimitiveOrListObjectEntity ParseHeaders(HttpResponseHeaders responseHeaders, 
        HttpContentHeaders contentHeaders)
    {
        var result = new PrimitiveOrListObjectEntity();
        ParseHeadersImpl(responseHeaders, result);
        ParseHeadersImpl(contentHeaders, result);
        return result;
    }

    private static void ParseHeadersImpl(HttpHeaders headers, PrimitiveOrListObjectEntity result)
    {
        foreach (var header in headers)
        {
            var headerEntity = new PrimitiveOrList();
            var value = header.Value.ToList();
            if (value.Count() > 1)
            {
                var list = new PrimitiveList();
                foreach (var singleVal in value)
                {
                    var primitiveEntity = new PrimitiveEntity();
                    primitiveEntity.String = singleVal;
                    list.Value.Add(primitiveEntity);
                }

                headerEntity.List = list;
            }
            else
            {
                var stringEntity = new PrimitiveEntity();
                stringEntity.String = value.Single();
                headerEntity.Primitive = stringEntity;
            }
            result.Properties[header.Key] = headerEntity;
        }
    } 

    private static string? PrimitiveEntityToString(PrimitiveEntity entity)
    {
        switch (entity.ContentCase)
        {
            case PrimitiveEntity.ContentOneofCase.Boolean:
                return entity.Boolean.ToString();
            case PrimitiveEntity.ContentOneofCase.Decimal:
                return entity.Decimal.ToDecimal().ToString(CultureInfo.InvariantCulture);
            case PrimitiveEntity.ContentOneofCase.Integer:
                return entity.Integer.ToString();
            case PrimitiveEntity.ContentOneofCase.String:
                return entity.String;
            default: 
                return null;
        }
    }

    private static OpenApiResponse ResolveOpenApiResponse(OpenApiResponses responses, HttpResponseMessage message)
    {
        var stringCode = ((int)message.StatusCode).ToString();
        
        if (responses.TryGetValue(stringCode, out var response)) return response;
        throw new ApiRuntimeException("Invalid response " + stringCode);
    }
}