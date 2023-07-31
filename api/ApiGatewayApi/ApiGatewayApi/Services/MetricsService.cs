using ApiGatewayApi.ApiConfigs;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace ApiGatewayApi.Services;

public class MetricsService
{
    public static  readonly Summary ApiRequestTime = Metrics.CreateSummary("apigtw_api_request_time",
        "Total time of resolved frontend HTTP requests", "api_name", "api_version", "endpoint",
        "method", "status", "response_code");

    public static readonly Summary BackendRequestTime = Metrics.CreateSummary("apigtw_service_request_time",
        "Backend request time", "api_name", "api_version", "endpoint", "method", 
        "status", "response_code");

    private static readonly Counter ApiRequestErrorCount = Metrics.CreateCounter("apigtw_http_request_errors",
        "Number of errors on frontend requests", "path", "code", "status");

    private static readonly Counter BackendRequestErrorCount = Metrics.CreateCounter("apigtw_service_request_errors",
        "Number of errors on backend requests", "api_name", "api_version", "endpoint", "method", "code");
    public void RecordApiRequestTime(Summary target, ExecutionRequest request, ExecutionResponse response, TimeSpan time)
    {
        target.WithLabels(request.ApiName, request.ApiVersion, request.Path, request.Method, 
                "FINISHED", response.Status.ToString())
            .Observe(time.TotalMilliseconds);
    }

    public void RecordApiCorsRequestTime(ApiConfig api, string path, string method, TimeSpan timeSpan)
    {
        ApiRequestTime.WithLabels(api.GetMetadata().Name, api.GetMetadata().Version,
                path, method, "CORS_PREFLIGHT", "200")
            .Observe(timeSpan.TotalMilliseconds);
    }

    public void IncrementApiError(string path, string code, string status)
    {
        ApiRequestErrorCount.WithLabels(path, code, status).Inc();
    }

    public void IncrementBackendError(ExecutionRequest request, string code)
    {
        BackendRequestErrorCount.WithLabels(request.ApiName, request.ApiVersion, request.Path,
            request.Method, code);
    }
}