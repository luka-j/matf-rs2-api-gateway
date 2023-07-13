using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using ApiGatewayApi.Exceptions;

namespace ApiGatewayApi.Processing;

public class EntityMapper
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    public Entity MapToEntity(JsonNode node)
    {
        var entity = new Entity();
        switch (node)
        {
            case JsonObject jsonObject:
                entity.Object = ParseJsonObject(jsonObject);
                break;
            case JsonArray array:
                entity.List = ParseJsonArray(array);
                break;
            case JsonValue value:
                ParseJsonValue(value, entity);
                break;
        }
        return entity;
    }

    public JsonNode MapToJsonNode(Entity entity)
    {
        switch (entity.ContentCase)
        {
            case Entity.ContentOneofCase.Object:
                return ParseObjectEntity(entity.Object);
            case Entity.ContentOneofCase.List:
                return ParseListEntity(entity.List);
            case Entity.ContentOneofCase.Boolean:
                return JsonValue.Create(entity.Boolean);
            case Entity.ContentOneofCase.Decimal:
                return JsonValue.Create(entity.Decimal.ToDecimal());
            case Entity.ContentOneofCase.Integer:
                return JsonValue.Create(entity.Integer);
            case Entity.ContentOneofCase.String:
                return JsonValue.Create(entity.String);
        }
        _logger.Error("Cannot parse entity {Entity}. Invalid ContentCase: {ContentCase}", 
            entity, entity.ContentCase);
        throw new ApiRuntimeException("Cannot parse entity");
    }

    private ObjectEntity ParseJsonObject(JsonObject obj)
    {
        var entity = new ObjectEntity();
        foreach (var (key, value) in obj)
        {
            if (value != null)
            {
                entity.Properties[key] = MapToEntity(value);
            }
        }

        return entity;
    }

    private ListEntity ParseJsonArray(JsonArray array)
    {
        var entity = new ListEntity();
        foreach (var node in array)
        {
            if (node != null)
            {
                entity.Value.Add(MapToEntity(node));
            }
        }

        return entity;
    }

    private void ParseJsonValue(JsonValue val, Entity entity)
    {
        var element = val.GetValue<JsonElement>();
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                entity.String = element.GetString();
                break;
            case JsonValueKind.Number:
                entity.Decimal = element.GetDecimal();
                break;
            case JsonValueKind.True:
                entity.Boolean = true;
                break;
            case JsonValueKind.False:
                entity.Boolean = false;
                break;
            default:
                _logger.Warning("Unknown ValueKind: {ValueKind} for value {Value}. Skipping...", element.ValueKind, val);
                break;
        }
    }
    
    
    private JsonObject ParseObjectEntity(ObjectEntity entity)
    {
        var obj = new JsonObject();
        foreach (var (key, value) in entity.Properties)
        {
            obj.Add(key, MapToJsonNode(value));
        }

        return obj;
    }

    private JsonArray ParseListEntity(ListEntity entity)
    {
        var arr = new JsonArray();
        foreach (var item in entity.Value)
        {
            arr.Add(MapToJsonNode(item));
        }

        return arr;
    }
    
    // not sure why are there 5 different ways to represent HTTP headers in C#/ASP.NET
    // this should cover most of them
    public void MapToPrimitiveOrListObjectEntity(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, 
        PrimitiveOrListObjectEntity result)
    {
        foreach (var header in headers)
        {
            var headerEntity = new PrimitiveOrList();
            var value = header.Value.ToList();
            if (value.Count > 1)
            {
                var list = new PrimitiveList();
                foreach (var singleVal in value)
                {
                    var primitiveEntity = new PrimitiveEntity();
                    primitiveEntity.String = singleVal;
                    list.Value.Add(primitiveEntity);
                }

                headerEntity.List = list;
            }
            else
            {
                var stringEntity = new PrimitiveEntity();
                stringEntity.String = value.Single();
                headerEntity.Primitive = stringEntity;
            }
            result.Properties[header.Key] = headerEntity;
        }
    }
    
    public string? PrimitiveEntityToString(PrimitiveEntity entity)
    {
        switch (entity.ContentCase)
        {
            case PrimitiveEntity.ContentOneofCase.Boolean:
                return entity.Boolean.ToString();
            case PrimitiveEntity.ContentOneofCase.Decimal:
                return entity.Decimal.ToDecimal().ToString(CultureInfo.InvariantCulture);
            case PrimitiveEntity.ContentOneofCase.Integer:
                return entity.Integer.ToString();
            case PrimitiveEntity.ContentOneofCase.String:
                return entity.String;
            default: 
                return null;
        }
    }
}