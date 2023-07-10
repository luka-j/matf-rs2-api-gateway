using System.Text.Json;
using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Processing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Controllers;

[ApiController]
public class ExposedApisController : ControllerBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    private readonly RequestExecutor _requestExecutor;

    public ExposedApisController(RequestExecutor requestExecutor)
    {
        _requestExecutor = requestExecutor;
    }

    [Route("/{*path}")]
    public void Get(string path)
    {
        var response = HttpContext.Response;
        try
        {
            var now = DateTime.Now;
            _requestExecutor.ExecuteRequest(path, now, HttpContext);
            HttpContext.Response.CompleteAsync();
        }
        catch (HttpResponseException e)
        {
            response.StatusCode = e.ResponseCode;
            response.WriteAsJsonAsync(e.ResponseBody);
        }
    }
}