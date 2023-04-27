namespace ApiGatewayApi.Exceptions;

public class PathNotFound : HttpResponseException
{
    public PathNotFound(string message) : base("PATH_NOT_FOUND", message, 404)
    {
    }
}