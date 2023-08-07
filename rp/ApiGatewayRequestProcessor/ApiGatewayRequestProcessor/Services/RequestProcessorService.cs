using ApiGatewayApi;
using Grpc.Core;

namespace ApiGatewayRequestProcessor.Services;

public class RequestProcessorService : RequestProcessor.RequestProcessorBase
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    public override Task<ExecutionResponse> ProcessRequest(ExecutionRequest request, ServerCallContext context)
    {
        _logger.Information("Executing request {Request}", request);
        return base.ProcessRequest(request, context);
    }
}