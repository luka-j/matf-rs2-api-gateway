using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Processing;
using ApiGatewayApi.Services;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Controllers;

[ApiController]
public class ExposedApisController : ControllerBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    
    private readonly RequestExecutor _requestExecutor;
    private readonly ControllerUtils _utils;
    private readonly MetricsService _metrics;

    public ExposedApisController(RequestExecutor requestExecutor, ControllerUtils utils, MetricsService metrics)
    {
        _requestExecutor = requestExecutor;
        _utils = utils;
        _metrics = metrics;
    }

    [Route("/{*path}")]
    public async ValueTask Get(string path)
    {
        var now = DateTime.Now;
        var response = HttpContext.Response;
        try
        {
            await _requestExecutor.ExecuteRequest(path, now, new RequestMetadata
            {
                IpAddress = _utils.GetIp(HttpContext.Request),
                RequestId = _utils.GenerateRequestId(path),
                StartTime = now.ToString("O"),
            }, HttpContext);
        }
        catch (HttpResponseException e)
        {
            _metrics.IncrementApiError(path, e.ResponseCode.ToString(), e.ResponseBody.Code);
            response.StatusCode = e.ResponseCode;
            await response.WriteAsJsonAsync(e.ResponseBody);
            await response.CompleteAsync();
        }
        catch (Exception e)
        {
            _metrics.IncrementApiError(path, "500", "INTERNAL_SERVER_ERROR");
            _logger.Error(e, "An unexpected exception occurred");
            response.StatusCode = 500;
            await response.WriteAsJsonAsync(new HttpResponseException.ErrorResponse("INTERNAL_SERVER_ERROR",
                "An unexpected error has occured. Please try again later."));
            await response.CompleteAsync();
        }
    }
}