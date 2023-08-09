using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using Grpc.Core;
using Grpc.Net.Client;

namespace ApiGatewayRequestProcessor.Gateways;

public class ApiGateway
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly GrpcChannel _channel;
    
    private readonly HttpRequester.HttpRequesterClient _client;

    public ApiGateway()
    {
        _channel = GrpcChannel.ForAddress("http://api-svc:grpc");
        _client = new HttpRequester.HttpRequesterClient(_channel);
    }

    public async Task<ExecutionResponse> InvokeRequest(ExecutionRequest request)
    {
        try
        {
            var asyncResult = _client.MakeHttpRequestAsync(request);
            var response = await asyncResult.ResponseAsync;
            _logger.Debug("Got response from http requester {Response}", response);
            var headers = await asyncResult.ResponseHeadersAsync;
            if (asyncResult.GetStatus().StatusCode != StatusCode.OK)
            {
                _logger.Error("Error processing InvokeRequest: status {Status}, headers: {Headers}",
                    asyncResult.GetStatus(), headers);
                throw new ApiRuntimeException("Error invoking request");
            }

            return response;
        }
        catch (RpcException e)
        {
            _logger.Error(e, "RpcException while invoking request {Request}", request);
            throw new ApiRuntimeException("Error invoking request");
        }
    }
}