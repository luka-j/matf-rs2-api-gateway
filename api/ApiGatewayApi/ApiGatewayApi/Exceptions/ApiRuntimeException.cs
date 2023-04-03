namespace ApiGatewayApi.Exceptions;

public class ApiRuntimeException : Exception
{
    public ApiRuntimeException(string? message) : base(message)
    {
    }
}