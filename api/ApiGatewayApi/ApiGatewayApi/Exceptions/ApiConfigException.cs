namespace ApiGatewayApi.Exceptions;

public class ApiConfigException : Exception
{
    public ApiConfigException(string? message) : base(message)
    {
    }
}