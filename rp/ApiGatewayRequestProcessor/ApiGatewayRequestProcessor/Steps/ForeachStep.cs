using ApiGatewayApi;
using ApiGatewayRequestProcessor.Configs;
using ApiGatewayRequestProcessor.Exceptions;
using ApiGatewayRequestProcessor.Utils;
using Serilog;

namespace ApiGatewayRequestProcessor.Steps;

public class ForeachStep : Step
{
    private string _element;
    private string _list;
    
    public string Foreach { 
        set
        {
            var parts = value.Split(":");
            if (parts.Length != 2)
            {
                throw new ApiConfigException("Invalid foreach syntax, should contain single colon");
            }

            _element = parts[0].Trim();
            _list = parts[1].Trim();
        }
    }
    
    public List<Step> Do { get; set; }
    
    public override ObjectEntity Execute(ObjectEntity state)
    {
        var entity = state.Find(_list);
        if (entity is not { ContentCase: Entity.ContentOneofCase.List })
        {
            Log.Warning("Attempted to iterate over non-list {ListName}, is {List}", _list, entity);
            throw new ApiRuntimeException("Attempted to iterate over non-list " + _list);
        }

        var list = entity.List.Value;
        foreach (var element in list)
        {
            state.Insert(element, _element);
            state = ApiOperation.Execute(Do, state);
        }
        state.Delete(_element);
        
        return state;
    }
}