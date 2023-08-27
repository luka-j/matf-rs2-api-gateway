using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Gateways;

namespace ApiGatewayRequestProcessor;

public class Initializer
{

    private Serilog.ILogger _logger = Serilog.Log.Logger;
    
    public Initializer(ConfGateway confGateway, ConfigRepository configRepository)
    {
        try
        {
            _logger.Information("Initializing configs");
            var specs = confGateway.GetSpecs();
            foreach (var spec in specs.Specs_)
            {
                configRepository.UpdateConfig(spec.Data, DateTime.Now, false);
            }
            _logger.Information("Initialized {Count} configs", specs.Specs_.Count);
        }
        catch (Exception)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                _logger.Warning("Failed to initialize configs");
            }
            else
            {
                throw;
            }
        }
    }
}