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

                var from = fromTo[0].Trim();
                var to = fromTo[1].Trim();
                string? def = null;
                if (fromTo[0].Contains('/'))
                {
                    var fromParts = fromTo[0].Split("/");
                    from = fromParts[0].Trim();
                    def = fromParts[1].Trim();
                }
                if (!from.StartsWith("${") || !from.EndsWith("}"))
                {
                    from = "${" + from + "}";
                }
                if (!to.StartsWith("${") || !to.EndsWith("}"))
                {
                    to = "${" + to + "}";
                }
                _copyPairs.Add(new FromToPair
                {
                    From = from
                        .Replace("\\s", "/")
                        .Replace("\\c", ",")
                        .Replace("\\arr", "->")
                        .Replace("\\b", "\\"),
                    To = to,
                    Default = def
                });
            }
        }
    }

    public override Task<ObjectEntity> Execute(ObjectEntity state, Dictionary<string, List<Step>>? stepRepository)
    {
        foreach (var pair in _copyPairs)
        {
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
            else
            {
                throw new ApiRuntimeException("Attempted to copy from non-existing path " + pair.From);
            }
        }

        return Task.FromResult(state);
    }
}