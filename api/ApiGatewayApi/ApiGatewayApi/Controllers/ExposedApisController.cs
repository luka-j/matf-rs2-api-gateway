using ApiGatewayApi.Exceptions;
using ApiGatewayApi.Processing;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace ApiGatewayApi.Controllers;

[ApiController]
public class ExposedApisController : ControllerBase
{
    private readonly ILogger _logger = Serilog.Log.Logger;
    
    private readonly RequestExecutor _requestExecutor;
    private readonly ControllerUtils _utils;

    public ExposedApisController(RequestExecutor requestExecutor, ControllerUtils utils)
    {
        _requestExecutor = requestExecutor;
        _utils = utils;
    }

    [Route("/{*path}")]
    public async ValueTask Get(string path)
    {
        var response = HttpContext.Response;
        try
        {
            var now = DateTime.Now;
            await _requestExecutor.ExecuteRequest(path, now, new RequestMetadata
            {
                IpAddress = _utils.GetIp(HttpContext.Request),
                RequestId = _utils.GenerateRequestId(),
                StartTime = now.ToString("O"),
            }, HttpContext);
        }
        catch (HttpResponseException e)
        {
            response.StatusCode = e.ResponseCode;
            await response.WriteAsJsonAsync(e.ResponseBody);
            await response.CompleteAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e, "An unexpected exception occurred");
            response.StatusCode = 500;
            await response.WriteAsJsonAsync(new HttpResponseException.ErrorResponse("INTERNAL_SERVER_ERROR",
                "An unexpected error has occured. Please try again later."));
            await response.CompleteAsync();
        }
    }
}