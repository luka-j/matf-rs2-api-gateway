using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;

namespace ApiGatewayRequestProcessor.Steps;

public class CopyStep : Step
{
    private List<FromToPair> _copyPairs = new();

    public string Copy
    {
        set
        {
            var values = value.Split(',');
            foreach (var val in values)
            {
                var fromTo = val.Split("->");
                if (fromTo.Length != 2)
                {
                    throw new ApiConfigException("Invalid copy value: " + fromTo + ". Expected -> symbol somewhere.");
                }

                var from = fromTo[0];
                string? def = null;
                if (fromTo[0].Contains("/"))
                {
                    var fromParts = fromTo[0].Split("/");
                    from = fromParts[0].Trim();
                    def = fromParts[1].Trim();
                }
                _copyPairs.Add(new FromToPair
                {
                    From = from
                        .Replace("\\s", "/")
                        .Replace("\\arr", "->")
                        .Replace("\\b", "\\").Trim(),
                    To = fromTo[1].Trim(),
                    Default = def
                });
            }
        }
    }

    public override ObjectEntity Execute(ObjectEntity state)
    {
        return state;
    }
}