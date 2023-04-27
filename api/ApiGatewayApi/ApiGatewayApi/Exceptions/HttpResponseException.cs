namespace ApiGatewayApi.Exceptions;

public class HttpResponseException : Exception
{
    public HttpResponseException(string code, string message, int responseCode)
    {
        ResponseBody = new ErrorResponse(code, message);
        ResponseCode = responseCode;
    }

    public ErrorResponse ResponseBody { get;  }
    public int ResponseCode { get; }

    public record ErrorResponse(string Code, string Message);
}