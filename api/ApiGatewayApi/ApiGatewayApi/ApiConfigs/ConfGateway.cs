using ApiGatewayApi.Exceptions;
using Grpc.Core;
using Grpc.Net.Client;
using Retriever;

namespace ApiGatewayApi.ApiConfigs;

public class ConfGateway
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly GrpcChannel _channel;
    
    private readonly ConfigRetriever.ConfigRetrieverClient _client;
    
    public ConfGateway()
    {
        _channel = GrpcChannel.ForAddress("http://conf-svc:5000");
        _client = new ConfigRetriever.ConfigRetrieverClient(_channel);
    }

    public Specs GetFrontends()
    {
        try {
            return _client.GetAllFrontendConfigs(new Retriever.Empty());
        }
        catch (RpcException e)
        {
            _logger.Error(e, "RpcException while invoking frontend config retrieve request");
            throw new ApiRuntimeException("Error invoking request");
        }
    }
    
    public Specs GetBackends()
    {
        try {
            return _client.GetAllBackendConfigs(new Retriever.Empty());
        }
        catch (RpcException e)
        {
            _logger.Error(e, "RpcException while invoking backend config retrieve request");
            throw new ApiRuntimeException("Error invoking request");
        }
    }
}