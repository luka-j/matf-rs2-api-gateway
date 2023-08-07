using System.Xml.XPath;
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
                if (fromTo[0].Contains('/'))
                {
                    var fromParts = fromTo[0].Split("/");
                    from = fromParts[0];
                    def = fromParts[1].Trim();
                }
                _copyPairs.Add(new FromToPair
                {
                    From = from
                        .Replace("\\s", "/")
                        .Replace("\\c", ",")
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
        foreach (var pair in _copyPairs)
        {
            if (!pair.From.StartsWith("${") || !pair.From.StartsWith("}"))
            {
                pair.From = "${" + pair.From + "}";
            }
            if (!pair.To.StartsWith("${") || !pair.To.StartsWith("}"))
            {
                pair.To = "${" + pair.To + "}";
            }

            var entity = state.Find(pair.From);
            if (entity != null)
            {
                state.Insert(entity, pair.To);
            }
            else if(pair.Default != null)
            {
                if (pair.Default.StartsWith("${") && pair.Default.EndsWith("}") &&
                    pair.Default.Count(c => c == '}') == 1)
                {
                    entity = state.Find(pair.Default);
                    if (entity != null)
                    {
                        state.Insert(entity, pair.To);
                    }
                    else
                    {
                        throw new ApiRuntimeException("Attempted to copy from non-existing path " + pair.From +
                                                      "(default " + pair.Default + ")!");
                    }
                }
                else
                {
                    entity = new Entity { String = state.Substitute(pair.Default) };
                    state.Insert(entity, pair.To);
                }
            }
        }

        return state;
    }
}