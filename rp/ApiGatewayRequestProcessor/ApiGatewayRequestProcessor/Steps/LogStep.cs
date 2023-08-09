using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace ApiGatewayRequestProcessor.Steps;

public class LogStep : Step
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    
    private LogEventLevel _level;
    private string _message;
    
    public string Log
    {
        set
        {
            var parts = value.Split(" ", 2);
            if (parts.Length != 2)
            {
                throw new ApiConfigException("Log value must contain at least two parts: level and message");
            }

            if (!Enum.TryParse(parts[0].Trim(), out _level))
            {
                throw new ApiConfigException("Invalid log level: " + parts[0]);
            }
            _message = parts[1].Trim();
        }
    }
    
    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        _logger.Write(_level, "{Message}", state.Substitute(_message));
        return Task.FromResult(state);
    }
}