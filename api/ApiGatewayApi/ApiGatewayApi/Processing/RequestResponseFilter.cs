using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Processing;

public class RequestResponseFilter
{
    public Entity FilterBody(OpenApiRequestBody spec, Entity data)
    {
        // todo
        return data;
    }
    
    public Entity FilterBody(OpenApiMediaType spec, Entity data)
    {
        // todo
        return data;
    }

    public Tuple<PrimitiveObjectEntity, PrimitiveOrListObjectEntity, PrimitiveOrListObjectEntity> FilterParams(
        IList<OpenApiParameter> spec, PrimitiveObjectEntity pathParams, PrimitiveOrListObjectEntity headerParams,
        PrimitiveOrListObjectEntity queryParams)
    {
        // todo
        return new Tuple<PrimitiveObjectEntity, PrimitiveOrListObjectEntity, PrimitiveOrListObjectEntity>(pathParams,
            headerParams, queryParams);
    }

    public PrimitiveOrListObjectEntity FilterHeaders(IList<OpenApiParameter> spec, PrimitiveOrListObjectEntity headers)
    {
        // todo
        return headers;
    }

    public PrimitiveOrListObjectEntity FilterHeaders(IDictionary<string, OpenApiHeader> spec, PrimitiveOrListObjectEntity headers)
    {
        // todo
        return headers;
    }
}