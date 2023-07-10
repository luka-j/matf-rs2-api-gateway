using ApiGatewayApi.ApiConfigs;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Processing;

public class RequestExecutor
{
    
    private readonly ApiRepository _apis;

    public RequestExecutor(ApiRepository apis)
    {
        _apis = apis;
    }

    public void ExecuteRequest(string path, DateTime now, HttpContext httpContext)
    {
        var operation = ResolveOperation(path, now, httpContext.Request);
        httpContext.Response.Headers["Operation-Id"] = operation.OperationId;
        httpContext.Response.StatusCode = 200;
        httpContext.Response.StartAsync();
    }

    private OpenApiOperation ResolveOperation(string path, DateTime now, HttpRequest request)
    {
        var pathSegments = path.Split('/', 3);
        if (pathSegments.Length < 3)
        {
            throw new PathNotFound("API not found");
        }

        var apiName = pathSegments[0];
        var apiVersion = pathSegments[1];
        var currentConfig = _apis.Frontends.GetCurrentConfig(new ApiIdentifier(apiName, apiVersion), now);
        if (currentConfig == null)
        {
            throw new PathNotFound("API " + apiName + "/" + apiVersion + " not found");
        }

        var oasOperation = currentConfig.ResolveOperation(request.Method, pathSegments[2]);
        if (oasOperation == null)
        {
            throw new PathNotFound("Could not find route for path " + path);
        }
        return oasOperation;
    }
}