using System.Text.Json.Nodes;

namespace ApiGatewayApi.Processing;

public class EntityMapper
{
    public Entity MapToEntity(JsonNode node)
    {
        return new Entity(); //todo
    }

    public JsonNode MapToBody(Entity entity)
    {
        return new JsonObject(); //todo
    }
}