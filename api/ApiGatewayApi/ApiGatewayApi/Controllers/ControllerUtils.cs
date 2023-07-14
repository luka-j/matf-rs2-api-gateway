namespace ApiGatewayApi.Controllers;

public class ControllerUtils
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

    public string GenerateRequestId()
    {
        var idChars = new char[12];
        for (int i = 0; i < idChars.Length; i++)
        {
            idChars[i] = idChars[_random.Next(idChars.Length)];
        }

        return new String(idChars);
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
}