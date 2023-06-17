using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Filters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Requester;

public class HttpRequester
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly ApiRepository _apiRepository;
    private readonly RequestResponseFilter _filter;
    private readonly IHttpClientFactory _httpClientFactory;

    public HttpRequester(RequestResponseFilter filter, ApiRepository apiRepository, IHttpClientFactory httpClientFactory)
    {
        _filter = filter;
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

        OperationType operationType = OperationType.Get;
        if (!Enum.TryParse(request.Method, true, out operationType))
        {
            throw new BadRequestException("Invalid method");
        }

        var operation = apiConfig.Spec.Paths[request.Path]?.Operations[operationType];
        if (operation == null)
        {
            throw new PathNotFound("Path not found");
        }

        var requestBodyEntity = _filter.FilterBody(operation.RequestBody, request.RequestBody);
        var (pathParams, headerParams, queryParams) = 
            _filter.FilterParams(operation.Parameters, request.PathParameters, request.HeaderParameters, 
                request.QueryParameters);

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(apiConfig.Spec.Servers[0].Url);
        var httpMessage = MakeHttpRequestMessage(request.Method, request.Path, requestBodyEntity, pathParams,
            headerParams, queryParams);
        
        var response = await httpClient.SendAsync(httpMessage);
        // todo response parsing

        return new ExecutionResponse();
    }

    private HttpRequestMessage MakeHttpRequestMessage(string method, string path, Entity requestBody,
        PrimitiveObjectEntity pathParams, PrimitiveObjectEntity headers, PrimitiveOrListObjectEntity queryParams)
    {
        var message = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = BuildUri(path, pathParams, queryParams),
            Content = { Headers = {  }}
        };
        PopulateContent(message, requestBody, headers);
        return message;
    }

    private Uri BuildUri(string path, PrimitiveObjectEntity pathParams, PrimitiveOrListObjectEntity queryParams)
    {
        var parsedPath = ReplacePathParams(path, pathParams);
        return new Uri(QueryHelpers.AddQueryString(parsedPath, GetQueryParams(queryParams)));
    }

    private void PopulateContent(HttpRequestMessage message, Entity? body, PrimitiveObjectEntity headers)
    {
        if (body != null)
        {
            var content = new StringContent("");
            message.Content = content;
        }

        PopulateHeaders(headers, message.Content.Headers);
    }

    private void PopulateHeaders(PrimitiveObjectEntity headerParams, HttpContentHeaders headers)
    {
        foreach (var (key, value) in headerParams.Properties)
        {
            switch (value.ContentCase)
            {
                case PrimitiveEntity.ContentOneofCase.Boolean:
                    headers.Add(key, value.Boolean.ToString());
                    break;
                case PrimitiveEntity.ContentOneofCase.Decimal:
                    headers.Add(key, value.Decimal.ToDecimal().ToString(CultureInfo.InvariantCulture));
                    break;
                case PrimitiveEntity.ContentOneofCase.Integer:
                    headers.Add(key, value.Integer.ToString());
                    break;
                case PrimitiveEntity.ContentOneofCase.String:
                    headers.Add(key, value.String);
                    break;
            }
        }
    }

    private string ReplacePathParams(string path, PrimitiveObjectEntity pathParams)
    {
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

    private List<KeyValuePair<string,StringValues>> GetQueryParams(PrimitiveOrListObjectEntity queryParams)
    {
        return new List<KeyValuePair<string, StringValues>>(); // todo
    }
}