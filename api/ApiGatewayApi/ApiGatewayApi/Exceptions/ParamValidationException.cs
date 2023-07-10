namespace ApiGatewayApi.Exceptions;

public class ParamValidationException : Exception
{
    public ParamValidationException(string? message) : base(message)
    {
    }
}