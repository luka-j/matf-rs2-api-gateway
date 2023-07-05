using System.Text.Json.Nodes;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Filters;

public class RequestResponseFilter
{
    public Entity FilterBody(OpenApiRequestBody spec, Entity data)
    {
        // todo
        return data;
    }

    public Entity FilterBody(OpenApiRequestBody spec, JsonNode data)
    {
        // todo
        return new Entity();
    }

    public Tuple<PrimitiveObjectEntity, PrimitiveObjectEntity, PrimitiveOrListObjectEntity> FilterParams(
        IList<OpenApiParameter> spec, PrimitiveObjectEntity pathParams, PrimitiveObjectEntity headerParams,
        PrimitiveOrListObjectEntity queryParams)
    {
        // todo
        return new Tuple<PrimitiveObjectEntity, PrimitiveObjectEntity, PrimitiveOrListObjectEntity>(pathParams,
            headerParams, queryParams);
    }

    public PrimitiveObjectEntity FilterHeaders(IList<OpenApiParameter> spec, PrimitiveObjectEntity headers)
    {
        // todo
        return headers;
    }
}