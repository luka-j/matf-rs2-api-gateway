using ApiGatewayApi.Exceptions;
using Grpc.Core;
using Grpc.Net.Client;

namespace ApiGatewayApi.Processing;
 
/// <summary>
/// Make requests to request processor microservice.
///
/// If MOCK_GRPC environment variable is set, it bypasses the RP microservice and instead
/// routes requests directly to backend (it invokes itself via gRPC), acting essentially
/// as a reverse proxy if backends and frontends are identical.
/// </summary>
public class RequestProcessorGateway
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly GrpcChannel _channel;
    private readonly RequestProcessor.RequestProcessorClient _client;
    
    private readonly ApiGatewayApi.HttpRequester.HttpRequesterClient _httpRequesterClient;
    
    public RequestProcessorGateway()
    {
        if (Environment.GetEnvironmentVariable("MOCK_GRPC") == null)
        {
            _channel = GrpcChannel.ForAddress("http://localhost:5001");
            _client = new RequestProcessor.RequestProcessorClient(_channel);
        }
        else
        {
            _logger.Warning("Mocking gRPC calls to RequestProcessor...");
            _channel = GrpcChannel.ForAddress("http://localhost:5000");
            _httpRequesterClient = new ApiGatewayApi.HttpRequester.HttpRequesterClient(_channel);
        }
    }
    
    public async Task<ExecutionResponse> ProcessRequest(ExecutionRequest request)
    {
        try
        {
            AsyncUnaryCall<ExecutionResponse>? asyncResult;
            if (Environment.GetEnvironmentVariable("MOCK_GRPC") == null)
            {
                _logger.Debug("Making request to request processor: {Request}", request);
                asyncResult = _client.ProcessRequestAsync(request);
            }
            else
            {
                asyncResult = _httpRequesterClient.MakeHttpRequestAsync(request);
                _logger.Information("Got response from mocked middleware");
            }

            var response = await asyncResult.ResponseAsync;
            _logger.Debug("Got response from request processor {Response}", response);
            var headers = await asyncResult.ResponseHeadersAsync;
            if (asyncResult.GetStatus().StatusCode != StatusCode.OK)
            {
                _logger.Error("Error processing ProcessRequest: status {Status}, headers: {Headers}",
                    asyncResult.GetStatus(), headers);
                throw new ApiRuntimeException("Error processing request");
            }

            return response;
        }
        catch (RpcException e)
        {
            _logger.Error(e, "RpcException while processing request {Request}", request);
            throw new ApiRuntimeException("Error processing request");
        }
    }
}