﻿using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Gateways;
using ApiGatewayRequestProcessor.Utils;
using CCO;

namespace ApiGatewayRequestProcessor.Steps;

public class QueueStep : Step
{
    private static readonly CcoGateway CcoGateway = CcoGateway.Instance;

    private enum QueueOperation
    {
        Send
    }

    private QueueOperation _operation;
    public string Cache
    {
        set
        {
            if (!Enum.TryParse(value.ToUpper(), true, out _operation))
            {
                throw new ApiConfigException("Invalid queue operation: must be Send");
            }
        }
    }
    
    public string Name { get; set; }
    public string Message { get; set; }

    private string _name, _version;
    public string Id
    {
        set
        {
            var parts= value.Split("/");
            if (parts.Length != 2)
            {
                throw new ApiConfigException("Database id must be in form 'name/version'");
            }

            _name = parts[0];
            _version = parts[0];
        }
    }


    public override async Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        var configId = new ConfigIdentifier
        {
            ApiName = _name,
            ApiVersion = _version,
        };

        await CcoGateway.QueueSend(new QueueWriteRequest
        {
            Identifier = configId,
            Message = state.Substitute(Message),
            QueueName = state.Substitute(Name)
        });

        return state;
    }
}