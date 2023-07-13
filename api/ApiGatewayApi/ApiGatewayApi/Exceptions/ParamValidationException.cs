namespace ApiGatewayApi.Exceptions;

public class ParamValidationException : HttpResponseException
{
    public ParamValidationException(string message) : base("PARAM_VALIDATION_ERROR", message, 400)
    {
    }
}