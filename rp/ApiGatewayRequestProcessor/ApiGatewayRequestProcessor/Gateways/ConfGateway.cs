using ApiGatewayRequestProcessor.Exceptions;
using Grpc.Core;
using Grpc.Net.Client;
using Retriever;

namespace ApiGatewayRequestProcessor.Gateways;

public class ConfGateway
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly GrpcChannel _channel;
    
    private readonly ConfigRetriever.ConfigRetrieverClient _client;
    
    public ConfGateway()
    {
        _channel = GrpcChannel.ForAddress("http://conf-service:5000");
        _client = new ConfigRetriever.ConfigRetrieverClient(_channel);
    }

    public Specs GetSpecs()
    {
        try {
            return _client.GetAllRpConfigs(new Empty());
        }
        catch (RpcException e)
        {
            _logger.Error(e, "RpcException while invoking config retrieve request");
            throw new ApiRuntimeException("Error invoking request");
        }
    }
}