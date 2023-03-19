using Grpc.Core;

namespace ApiGatewayApi.Services;

public class HttpRequesterService : HttpRequester.HttpRequesterBase
{
    private readonly ILogger<HttpRequesterService> _logger;

    public HttpRequesterService(ILogger<HttpRequesterService> logger)
    {
        _logger = logger;
    }

    public override Task<ExecutionResponse> MakeHttpRequest(ExecutionRequest request, ServerCallContext context)
    {
        return Task.FromResult(new ExecutionResponse
        {
            Status = 200
        });
    }
}