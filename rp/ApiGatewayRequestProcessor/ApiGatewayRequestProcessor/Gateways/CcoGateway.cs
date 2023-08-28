using ApiGatewayRequestProcessor.Exceptions;
using CCO;
using Grpc.Core;
using Grpc.Net.Client;

namespace ApiGatewayRequestProcessor.Gateways;

public class CcoGateway
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly GrpcChannel _channel;
    
    private readonly DatasourceOperations.DatasourceOperationsClient _client;

    public static readonly CcoGateway Instance = new();
    
    public CcoGateway()
    {
        _channel = GrpcChannel.ForAddress("http://cco-service:80");
        _client = new DatasourceOperations.DatasourceOperationsClient(_channel);
    }

    public async Task<DatabaseReadResponse> DatabaseRead(DatabaseReadRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.DatabaseReadAsync(r));
    }
    
    public async Task<DatabaseWriteResponse> DatabaseWrite(DatabaseWriteRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.DatabaseWriteAsync(r));
    }
    
    public async Task<DatabaseDeleteResponse> DatabaseDelete(DatabaseDeleteRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.DatabaseDeleteAsync(r));
    }
    
    public async Task<CacheReadResponse> CacheRead(CacheReadRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.CacheReadAsync(r));
    }
    
    public async Task<CacheWriteResponse> CacheWrite(CacheWriteRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.CacheWriteAsync(r));
    }
    
    public async Task<QueueWriteResponse> QueueSend(QueueWriteRequest request)
    {
        return await InvokeCcoOperation(request, r => _client.QueueWriteAsync(r));
    }
    
    private async Task<TR> InvokeCcoOperation<TP, TR>(TP request, Func<TP, AsyncUnaryCall<TR>> callGrpcMethod)
    {
        try
        {
            var asyncResult = callGrpcMethod.Invoke(request);
            var response = await asyncResult.ResponseAsync;
            _logger.Debug("Got response from CCO {Response}", response);
            var headers = await asyncResult.ResponseHeadersAsync;
            if (asyncResult.GetStatus().StatusCode != StatusCode.OK)
            {
                _logger.Error("Error processing InvokeCcoOperation: status {Status}, headers: {Headers}",
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