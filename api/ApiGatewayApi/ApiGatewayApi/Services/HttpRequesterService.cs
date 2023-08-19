using ApiGatewayApi.Exceptions;
using Grpc.Core;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Services;

public class HttpRequesterService : HttpRequester.HttpRequesterBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly Processing.HttpRequester _requester;
    private readonly MetricsService _metrics;

    public HttpRequesterService(Processing.HttpRequester requester, MetricsService metrics)
    {
        _requester = requester;
        _metrics = metrics;
    }

    public override Task<ExecutionResponse> MakeHttpRequest(ExecutionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            LogContext.PushProperty("CorrelationId", request.RequestMetadata.RequestId);
            _logger.Information("MakeHttpRequest request: {Request}", request);
            try
            {
                return _requester.MakeRequest(request);
            }
            catch (PathNotFound e)
            {
                _metrics.IncrementBackendError(request, "NotFound");
                throw new RpcException(new Status(StatusCode.NotFound, e.Message));
            }
            catch (BadRequestException e)
            {
                _metrics.IncrementBackendError(request, "FailedPrecondition");
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }
            catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
            {
                _metrics.IncrementBackendError(request, "FailedPrecondition");
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid request data"));
            }
            catch (Exception e)
            {
                _metrics.IncrementBackendError(request, "Unknown");
                _logger.Error(e, "Error while making backend call for request {Request}", request);
                throw;
            }
        });
    }
}