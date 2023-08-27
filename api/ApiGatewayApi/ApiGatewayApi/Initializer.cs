using ApiGatewayApi.ApiConfigs;

namespace ApiGatewayApi;

public class Initializer
{

    private Serilog.ILogger _logger = Serilog.Log.Logger;
    
    public Initializer(ConfGateway confGateway, ApiRepository configRepository)
    {
        try
        {
            _logger.Information("Initializing configs");
            var frontendSpecs = confGateway.GetFrontends();
            foreach (var spec in frontendSpecs.Specs_)
            {
                configRepository.Frontends.AddConfig(new ApiSpec(spec.Data, DateTime.Now), false);
            }
            _logger.Information("Initialized {Count} frontend configs", frontendSpecs.Specs_.Count);
            
            var backendSpecs = confGateway.GetFrontends();
            foreach (var spec in backendSpecs.Specs_)
            {
                configRepository.Backends.AddConfig(new ApiSpec(spec.Data, DateTime.Now), false);
            }
            _logger.Information("Initialized {Count} backend configs", backendSpecs.Specs_.Count);
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