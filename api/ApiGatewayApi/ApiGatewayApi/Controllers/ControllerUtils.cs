using System.Text.RegularExpressions;
using ApiGatewayApi.ApiConfigs;
using Serilog.Context;

namespace ApiGatewayApi.Controllers;

public partial class ControllerUtils
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    private readonly string[] IP_HEADERS = { "CF-Connecting-IP", "X-Forwarded-For", "X-Real-Ip", "Forwarded" };
    private Random _random = new();
    
    public string GetIp(HttpRequest httpRequest)
    {
        foreach (var headerName in IP_HEADERS)
        {
            if (TryGetHeaderValue(httpRequest, headerName, out var header)) return header!;
        }

        var httpConnection = httpRequest.HttpContext.Connection;
        return httpConnection.RemoteIpAddress + ":" + httpConnection.RemotePort;
    }

    public string GenerateRequestId(string path = "")
    {
        var prefix = "";
        if (path is not { Length: 0 })
        {
            if (path.Contains('_') || path.Contains(' ') || path.Contains('-'))
            {
                var parts = PathSplittingRegex().Split(path);
                for (var i = 0; i < 3 && i < parts.Length; i++)
                {
                    prefix += parts[i].ToUpper();
                }
            }
            else if (path.Length < 3)
            {
                prefix = path.ToUpper();
            }
            else
            {
                prefix = path[..3].ToUpper();
            }
        }
        var idChars = new char[12];
        for (var i = 0; i < idChars.Length; i++)
        {
            idChars[i] = Chars[_random.Next(Chars.Length)];
        }

        var requestId = prefix + "_" + new string(idChars);
        LogContext.PushProperty("CorrelationId", requestId);
        return requestId;
    }

    public void AddCorsHeadersToResponse(HttpContext httpContext, ApiConfig config)
    {
        var requestOrigin = httpContext.Request.Headers.Origin;
        if (requestOrigin.Count == 0 || requestOrigin[0] == "null") return;
        var originUri = new Uri(requestOrigin[0]!); // Browsers always send a single origin header
        
        foreach (var openApiServer in config.Spec.Servers)
        {
            if (!Uri.TryCreate(openApiServer.Url, UriKind.Absolute, out var uri)) continue;
            
            if (uri.Scheme == originUri.Scheme && uri.Host == originUri.Host && uri.Port == originUri.Port)
            { 
                httpContext.Response.Headers.AccessControlAllowOrigin = requestOrigin;
                httpContext.Response.Headers.AccessControlAllowHeaders = "*";
                httpContext.Response.Headers.AccessControlAllowMethods = "*";
                break;
            }
        }
    }

    private bool TryGetHeaderValue(HttpRequest httpRequest, string header, out string? value)
    {
        if (httpRequest.Headers.TryGetValue(header, out var vals))
        {
            if (vals.Count != 1)
            {
                _logger.Warning("Multiple {Header} headers: {Vals}", header, vals);
            }

            value = vals[0];
            return true;
        }

        value = null;
        return false;
    }

    [GeneratedRegex("[ -_]")]
    private static partial Regex PathSplittingRegex();
}