using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;

namespace ApiGatewayRequestProcessor.Steps;

public class HttpStep : Step
{
    private string _api;
    private string _version;
    private string _resource;
    private string _operation;

    public string Http
    {
        set
        {
            var parts = value.Split(" ");
            if (parts.Length != 4)
            {
                throw new ApiConfigException(
                    "Http step expression is invalid, should be 'api version resource operation'");
            }

            (_api, _version, _resource, _operation) = (parts[0], parts[1], parts[2], parts[3]);
        }
    }
    
    public string Body { get; set; }
    public string Headers { get; set; }
    public string QueryParams { get; set; }
    public string PathParams { get; set; }
    public int Timeout { get; set; }
    public int Retries { get; set; }


    public override ObjectEntity Execute(ObjectEntity state)
    {
        return state;
    }

}