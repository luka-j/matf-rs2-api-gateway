using ApiGatewayApi.Exceptions;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Services;

public class HttpRequesterService : HttpRequester.HttpRequesterBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly Requester.HttpRequester _requester;

    public HttpRequesterService(Requester.HttpRequester requester)
    {
        _requester = requester;
    }

    public override Task<ExecutionResponse> MakeHttpRequest(ExecutionRequest request, ServerCallContext context)
    {
        return Task.Run(() =>
        {
            _logger.Information("MakeHttpRequest request: {}", request);
            try
            {
                return _requester.MakeRequest(request);
            }
            catch (PathNotFound e)
            {
                throw new RpcException(new Status(StatusCode.NotFound, e.Message));
            }
            catch (BadRequestException e)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, e.Message));
            }
            catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid request data"));
            }
        });
    }
}