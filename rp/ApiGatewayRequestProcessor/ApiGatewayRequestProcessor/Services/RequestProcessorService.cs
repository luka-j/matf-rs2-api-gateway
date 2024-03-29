﻿using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using Grpc.Core;
using Serilog.Context;

namespace ApiGatewayRequestProcessor.Services;

public class RequestProcessorService : RequestProcessor.RequestProcessorBase
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;

    private readonly ApiGateway _apiGateway;
    private readonly ConfigRepository _repository;

    public RequestProcessorService(ConfigRepository repository, ApiGateway apiGateway)
    {
        _repository = repository;
        _apiGateway = apiGateway;
    }

    public override async Task<ExecutionResponse> ProcessRequest(ExecutionRequest request, ServerCallContext context)
    {
        LogContext.PushProperty("CorrelationId", request.RequestMetadata.RequestId);
        _logger.Information("Executing request {Request}", request);
        var now = DateTime.Parse(request.RequestMetadata.StartTime, null,
            System.Globalization.DateTimeStyles.RoundtripKind);
        var spec = _repository.GetCurrentConfig(new ApiIdentifier(request.ApiName, request.ApiVersion), now);
        if (spec == null)
        {
            _logger.Warning("Cannot find API {ApiName}/{ApiVersion} in config repository",
                request.ApiName, request.ApiVersion);
            throw new RpcException(new Status(StatusCode.NotFound, "API not found"));
        }

        var method = request.Method.ToLower();
        if (!spec.HasOperation(request.Path, method))
        {
            _logger.Warning("Cannot find operation {Method} {Path} defined in API {ApiName}/{ApiVersion}",
                method, request.Path, request.ApiName, request.ApiVersion);
            throw new RpcException(new Status(StatusCode.NotFound, "Operation not found"));
        }

        try
        {
            return await spec.Execute(request.Path, method, request, _apiGateway, _repository, now);
        }
        catch (ApiRuntimeException e)
        {
            _logger.Error(e, "Unexpected error occurred while executing request {Request}", request);
            throw new RpcException(new Status(StatusCode.Unknown, "Unexpected error: " + e.Message));
        }
    }
}