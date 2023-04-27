using System.Text.Json;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Controllers;

[ApiController]
public class ExposedApisController : ControllerBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    
    private readonly ApiRepository apis;

    public ExposedApisController(ApiRepository apis)
    {
        this.apis = apis;
    }

    [Route("/{*path}")]
    public void Get(string path)
    {
        var response = HttpContext.Response;
        try
        {
            var now = DateTime.Now;
            var operation = ResolveOperation(path, now);
            HttpContext.Response.Headers["Operation-Id"] = operation.OperationId;
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.StartAsync();
        }
        catch (HttpResponseException e)
        {
            response.StatusCode = e.ResponseCode;
            response.WriteAsJsonAsync(e.ResponseBody);
        }
    }

    private OpenApiOperation ResolveOperation(string path, DateTime now)
    {
        var pathSegments = path.Split('/', 3);
        if (pathSegments.Length < 3)
        {
            throw new PathNotFound("API not found");
        }

        var apiName = pathSegments[0];
        var apiVersion = pathSegments[1];
        var currentConfig = apis.Frontends.GetCurrentConfig(new ApiIdentifier(apiName, apiVersion), now);
        if (currentConfig == null)
        {
            throw new PathNotFound("API " + apiName + "/" + apiVersion + " not found");
        }

        var oasOperation = currentConfig.ResolveOperation(HttpContext.Request.Method, pathSegments[3]);
        if (oasOperation == null)
        {
            throw new PathNotFound("Could not find route for path " + path);
        }
        return oasOperation;
    }
}