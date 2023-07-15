using System.Globalization;
using System.Text.RegularExpressions;
using ApiGatewayApi.Exceptions;
using Microsoft.OpenApi.Models;

namespace ApiGatewayApi.Processing;

public partial class RequestResponseFilter
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;
    
    [GeneratedRegex("^[a-zA-Z0-9\\+/]*={0,2}$")]
    private static partial Regex Base64Regex();

    public Entity? FilterBody(OpenApiRequestBody? spec, Entity? data)
    {
        if (data == null)
        {
            if (spec != null && spec.Required) throw new ParamValidationException("body is null, but it's required by the spec");
            return null;
        }

        if (spec == null || !spec.Content.TryGetValue("application/json", out var mediaType))
        {
            _logger.Warning("Missing application/json content type for request body: {@Spec}", spec);
            return new Entity();
        }

        return FilterSchema(mediaType.Schema, data);
    }
    
    public Entity? FilterBody(OpenApiResponse spec, Entity? data)
    {
        if (data == null)
        {
            if (spec.Content.ContainsKey("application/json")) 
                throw new ParamValidationException("body is null, but it's required by the spec");
            return null;
        }
        
        if (!spec.Content.TryGetValue("application/json", out var mediaType))
        {
            _logger.Warning("Missing application/json content type for request body: {@Spec}", spec);
            return new Entity();
        }
        
        return FilterSchema(mediaType.Schema, data);
    }

    private Entity FilterSchema(OpenApiSchema spec, Entity data)
    {
        if (spec.AllOf != null && spec.AllOf.Any())
        {
            throw new ApiConfigException("allOf schemas are not supported!");
        }

        if (spec.OneOf != null && spec.OneOf.Any())
        {
            return ValidateOneOf(spec.OneOf, data);
        }

        if (spec.AnyOf != null && spec.AnyOf.Any())
        {
            return ValidateAnyOf(spec.AnyOf, data);
        }

        if (spec.AdditionalProperties != null)
        {
            _logger.Warning("additionalProperties are not supported and will be discarded");
        }
        
        switch (spec.Type)
        {
            case null:
                return data; // per OAS3, schema without a type matches any type; no filtering is wanted
            case "object":
                return FilterObject(spec.Properties, spec.Required, data);
            case "array":
                return FilterArray(spec, data);
            case "string":
                return ValidateString(spec, data);
            case "integer":
                return ValidateInteger(spec, data);
            case "number":
                return ValidateNumber(spec, data);
            case "boolean":
                return ValidateBoolean(spec, data);
            default:
                _logger.Warning("Unsupported spec type {Type}, returning empty data", spec.Type);
                return new Entity();
        }

    }

    private Entity ValidateOneOf(ICollection<OpenApiSchema> schemas, Entity data)
    {
        var validated = false;
        var result = new Entity();
        if (schemas.Count == 0)
        {
            return data;
        }
        foreach (var schema in schemas)
        {
            try
            {
                result = FilterSchema(schema, data);
                if (!validated)
                {
                    validated = true;
                }
                else
                {
                    throw new ParamValidationException("Failed to validate oneOf schema: multiple schemas match!");
                }
            }
            catch (ParamValidationException)
            {
                _logger.Debug("oneOf: Failed to validate {Data} against {@Schema}", data, schema);
            }
        }

        if (validated)
        {
            return result;
        }

        throw new ParamValidationException("Failed to validate oneOf schema: no schemas match!");
    }
    
    private Entity ValidateAnyOf(ICollection<OpenApiSchema> schemas, Entity data)
    {
        if (schemas.Count == 0)
        {
            return data;
        }
        foreach (var schema in schemas)
        {
            try
            {
                return FilterSchema(schema, data);
            }
            catch (ParamValidationException)
            {
                _logger.Debug("anyOf: Failed to validate {Data} against {@Schema}", data, schema);
            }
        }

        throw new ParamValidationException("Failed to validate anyOf schema: no schemas match!");
    }

    private Entity FilterObject(IDictionary<string, OpenApiSchema> spec, ISet<string> requiredFields, Entity data)
    {
        if (data.ContentCase != Entity.ContentOneofCase.Object)
        {
            throw new ParamValidationException("Expected object, got " + data.ContentCase);
        }

        var objectEntity = data.Object;
        var presentFields = requiredFields.Where(field => objectEntity.Properties.ContainsKey(field));
        var missingFields = requiredFields.Except(presentFields);
        if (missingFields.Any())
        {
            throw new ParamValidationException("Missing required fields: " + missingFields);
        }

        var ret = new Entity();
        var retObj = new ObjectEntity();

        foreach (var (key, value) in spec)
        {
            if (objectEntity.Properties.TryGetValue(key, out var property))
            {
                retObj.Properties.Add(key, FilterSchema(value, property));
            }
        }
        
        ret.Object = retObj;
        return ret;
    }

    private Entity FilterArray(OpenApiSchema spec, Entity data)
    {
        if (data.ContentCase != Entity.ContentOneofCase.List)
        {
            throw new ParamValidationException("Expected list, got " + data.ContentCase);
        }

        if (spec.MinItems != null && data.List.Value.Count < spec.MinItems)
        {
            throw new ParamValidationException("List must have at least " + spec.MinItems + " items, got: " +
                                               data.List.Value);
        }
        if (spec.MaxItems != null && data.List.Value.Count > spec.MaxItems)
        {
            throw new ParamValidationException("List must have at most " + spec.MaxItems + " items, got: " +
                                               data.List.Value);
        }

        var entityList = data.List;
        var ret = new Entity();
        var retList = new ListEntity();
        foreach (var entity in entityList.Value)
        {
            retList.Value.Add(FilterSchema(spec.Items, entity));
        }

        ret.List = retList;
        return ret;
    }

    private Entity ValidateString(OpenApiSchema spec, Entity data)
    {
        var value = data.ContentCase switch {
            Entity.ContentOneofCase.String => data.String,
            Entity.ContentOneofCase.Integer => data.Integer.ToString(),
            Entity.ContentOneofCase.Decimal => data.Decimal.ToDecimal().ToString(CultureInfo.InvariantCulture),
            Entity.ContentOneofCase.Boolean => data.Boolean.ToString(),
            _ => throw new ParamValidationException("Unsupported content type for string: " + data.ContentCase)
        };

        if (spec.Enum != null && spec.Enum.Any())
        {
            _logger.Warning("Enums are currently unsupported, not checking values");
        }
        
        ValidateStringValue(spec, value);

        return new Entity
        {
            String = value
        };
    }

    private void ValidateStringValue(OpenApiSchema spec, string value)
    {
        if (spec.MinLength != null && value.Length < spec.MinLength)
        {
            throw new ParamValidationException("String " + value + " is shorter than MinLength " +
                                               spec.MinLength);
        }

        if (spec.MaxLength != null && value.Length > spec.MaxLength)
        {
            throw new ParamValidationException("String " + value + " is longer than MaxLength " + spec.MaxLength);
        }

        if (spec.Format != null)
        {
            switch (spec.Format)
            {
                case "date":
                    if (!DateOnly.TryParse(value, out _))
                    {
                        throw new ParamValidationException("String " + value + " cannot be parsed to a date!");
                    }
                    break;
                case "date-time":
                    if (!DateTime.TryParse(value, out _))
                    {
                        throw new ParamValidationException("String " + value + " cannot be parsed to a date!");
                    }
                    break;
                case "base64":
                    if (!Base64Regex().IsMatch(value) && value.Length % 4 == 0)
                    {
                        throw new ParamValidationException("String " + value + " is not in base64 format!");
                    }
                    break;
                case "uuid":
                    if (!Guid.TryParse(value, out _))
                    {
                        throw new ParamValidationException("String " + value + " is not valid UUID!");
                    }
                    break;
                default:
                    _logger.Warning("Unsupported string format {Format}", spec.Format);
                    break;
            }
        }

        if (spec.Pattern != null && !Regex.IsMatch(value, spec.Pattern))
        {
            throw new ParamValidationException("String " + value + " does not match pattern " + spec.Pattern);
        }
    }
    
    private Entity ValidateInteger(OpenApiSchema schema, Entity data)
    {
        long value;
        switch (data.ContentCase)
        {
            case Entity.ContentOneofCase.Integer:
                value = data.Integer;
                break;
            case Entity.ContentOneofCase.Boolean:
                value = data.Boolean ? 1 : 0;
                break;
            case Entity.ContentOneofCase.Decimal:
                if (data.Decimal.ToDecimal().Scale == 0)
                {
                    value = Decimal.ToInt64(data.Decimal.ToDecimal());
                    break;
                }
                throw new ParamValidationException("Number " + data.Decimal.ToDecimal() +
                                                   " is not a valid integer");
            case Entity.ContentOneofCase.String:
                if (!long.TryParse(data.String, out value))
                {
                    throw new ParamValidationException("Cannot convert " + data.String + " to integer");
                }
                break;
            default:
                throw new ParamValidationException("Unsupported content type for integer" + data.ContentCase);
        }
        
        CheckNumericLimits(schema, value);

        if (schema.Format != null)
        {
            if (schema.Format.Equals("int32"))
            {
                if (value is > int.MaxValue or < int.MinValue)
                {
                    throw new ParamValidationException("Value " + value + " is out of bounds for format int32");
                }
            }
            // we're not supporting unbounded ints for now, so no need to check int64
        }

        return new Entity
        {
            Integer = value
        };
    }
    
    private Entity ValidateNumber(OpenApiSchema schema, Entity data)
    {
        decimal value;
        switch (data.ContentCase)
        {
            case Entity.ContentOneofCase.Integer:
                value = data.Integer;
                break;
            case Entity.ContentOneofCase.Boolean:
                value = data.Boolean ? 1 : 0;
                break;
            case Entity.ContentOneofCase.Decimal:
                value = data.Decimal.ToDecimal();
                break;
            case Entity.ContentOneofCase.String:
                if (!decimal.TryParse(data.String, out value))
                {
                    throw new ParamValidationException("Cannot convert " + data.String + " to decimal");
                }
                break;
            default:
                throw new ParamValidationException("Unsupported content type for decimal" + data.ContentCase);
        }
        
        CheckNumericLimits(schema, value);

        return new Entity
        {
            Decimal = DecimalEntity.FromDecimal(value)
        };
    }

    private void CheckNumericLimits(OpenApiSchema schema, decimal value)
    {
        if (schema.Minimum != null)
        {
            if (schema.ExclusiveMinimum.GetValueOrDefault(false) && value <= schema.Minimum)
            {
                throw new ParamValidationException("Value " + value + " is below (exclusive) minimum " + schema.Minimum);
            }

            if (!schema.ExclusiveMinimum.GetValueOrDefault(false) && value < schema.Minimum)
            {
                throw new ParamValidationException("Value " + value + " is below minimum " + schema.Minimum);
            }
        }
        if (schema.Maximum != null)
        {
            if (schema.ExclusiveMaximum.GetValueOrDefault(false) && value >= schema.Maximum)
            {
                throw new ParamValidationException("Value " + value + " is above (exclusive) maximum " + schema.Maximum);
            }

            if (!schema.ExclusiveMaximum.GetValueOrDefault(false) && value > schema.Maximum)
            {
                throw new ParamValidationException("Value " + value + " is above maximum " + schema.Maximum);
            }
        }
    }
    
    private Entity ValidateBoolean(OpenApiSchema schema, Entity data)
    {
        bool value;
        switch (data.ContentCase)
        {
            case Entity.ContentOneofCase.Integer:
                value = data.Integer != 0;
                break;
            case Entity.ContentOneofCase.Boolean:
                value = data.Boolean;
                break;
            case Entity.ContentOneofCase.Decimal:
                value = data.Decimal.ToDecimal() != 0;
                break;
            case Entity.ContentOneofCase.String:
                if (!bool.TryParse(data.String, out value))
                {
                    throw new ParamValidationException("Cannot convert " + data.String + " to bool");
                }
                break;
            default:
                throw new ParamValidationException("Unsupported content type for bool" + data.ContentCase);
        }

        return new Entity
        {
            Boolean = value
        };
    }

    public Tuple<PrimitiveObjectEntity, PrimitiveOrListObjectEntity, PrimitiveOrListObjectEntity> FilterParams(
        IEnumerable<OpenApiParameter> spec, PrimitiveObjectEntity pathParams, PrimitiveOrListObjectEntity headerParams,
        PrimitiveOrListObjectEntity queryParams)
    {
        var paramGroups = spec.GroupBy(param => param.In);
        foreach (var group in paramGroups)
        {
            switch (group.Key)
            {
                case ParameterLocation.Query:
                    queryParams = FilterPrimitiveOrListObject(group, queryParams);
                    break;
                case ParameterLocation.Path:
                    pathParams = FilterPrimitiveObject(group, pathParams);
                    break;
                case ParameterLocation.Header:
                    headerParams = FilterPrimitiveOrListObject(group, headerParams);
                    break;
                case ParameterLocation.Cookie:
                    _logger.Warning("Cookie parameters are not currently supported, ignoring");
                    break;
            }
        }
        return new Tuple<PrimitiveObjectEntity, PrimitiveOrListObjectEntity, PrimitiveOrListObjectEntity>(pathParams,
            headerParams, queryParams);
    }

    public PrimitiveOrListObjectEntity FilterPrimitiveOrListObject(
        IGrouping<ParameterLocation?, OpenApiParameter> specs, PrimitiveOrListObjectEntity data)
    {
        var requiredParams = specs
            .Where(spec => spec.Required)
            .Select(spec => spec.Name);
        var missingParams = requiredParams.Except(data.Properties.Keys);
        if (missingParams.Any())
        {
            throw new ParamValidationException("Missing params: " + missingParams);
        }
        
        var result = new PrimitiveOrListObjectEntity();
        foreach (var spec in specs)
        {
            if (data.Properties.TryGetValue(spec.Name, out var param))
            {
                result.Properties.Add(spec.Name, ValidatePrimitiveOrList(spec.Schema, param));
            }
        }
        return result;
    }

    public PrimitiveObjectEntity FilterPrimitiveObject(IGrouping<ParameterLocation?, OpenApiParameter> specs,
        PrimitiveObjectEntity data)
    {
        var requiredParams = specs
            .Where(spec => spec.Required)
            .Select(spec => spec.Name);
        var missingParams = requiredParams.Except(data.Properties.Keys);
        if (missingParams.Any())
        {
            throw new ParamValidationException("Missing params: " + missingParams);
        }
        
        var result = new PrimitiveObjectEntity();
        foreach (var spec in specs)
        {
            if (data.Properties.TryGetValue(spec.Name, out var param))
            {
                result.Properties.Add(spec.Name, ValidatePrimitive(spec.Schema, param));
            }
        }
        return result;
    }

    public PrimitiveOrListObjectEntity? FilterHeaders(IDictionary<string, OpenApiHeader>? spec, 
        PrimitiveOrListObjectEntity? headers)
    {
        if (spec == null && headers == null)
        {
            return null;
        }
        if (spec == null)
        {
            return new PrimitiveOrListObjectEntity();
        }
        var requiredHeaders = spec
            .Where(header => header.Value.Required)
            .Select(header => header.Key)
            .ToList();
        if (headers == null && requiredHeaders.Any())
        {
            throw new ParamValidationException("There are required headers, but none are provided");
        }
        if (headers == null)
        {
            return new PrimitiveOrListObjectEntity();
        }
        var missingHeaders = requiredHeaders.Except(headers.Properties.Keys);
        if (missingHeaders.Any())
        {
            throw new ParamValidationException("Missing headers " + missingHeaders);
        }

        var result = new PrimitiveOrListObjectEntity();
        foreach (var (key, value) in spec)
        {
            if (headers.Properties.TryGetValue(key, out var header))
            {
                var headerEntity = ValidatePrimitiveOrList(value.Schema, header);
                result.Properties.Add(key, headerEntity);
            }
        }
        return result;
    }

    private PrimitiveOrList ValidatePrimitiveOrList(OpenApiSchema spec, PrimitiveOrList data)
    {
        if (spec.AdditionalProperties != null)
        {
            _logger.Warning("additionalProperties are not supported and will be discarded");
        }

        Entity? primitiveEntityRepresentation = null;
        if (data.ContentCase == PrimitiveOrList.ContentOneofCase.Primitive)
        {
            primitiveEntityRepresentation = data.Primitive.ContentCase switch
            {
                PrimitiveEntity.ContentOneofCase.Boolean => new Entity { Boolean = data.Primitive.Boolean },
                PrimitiveEntity.ContentOneofCase.Decimal => new Entity { Decimal = data.Primitive.Decimal },
                PrimitiveEntity.ContentOneofCase.Integer => new Entity { Integer = data.Primitive.Integer },
                PrimitiveEntity.ContentOneofCase.String => new Entity { String = data.Primitive.String },
                _ => throw new ApiRuntimeException("Unexpected primitive content case " + data.Primitive.ContentCase)
            };
        }

        switch (spec.Type)
        {
            case "string":
                if (data.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                    throw new ParamValidationException("Cannot parse list " + data.List + " to primitive type");

                var validated = ValidateString(spec, primitiveEntityRepresentation!);
                return new PrimitiveOrList { Primitive = new PrimitiveEntity { String = validated.String } };
            case "integer":
                if (data.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                    throw new ParamValidationException("Cannot parse list " + data.List + " to primitive type");

                validated = ValidateInteger(spec, primitiveEntityRepresentation!);
                return new PrimitiveOrList { Primitive = new PrimitiveEntity { Integer = validated.Integer } };
            case "number":
                if (data.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                    throw new ParamValidationException("Cannot parse list " + data.List + " to primitive type");

                validated = ValidateNumber(spec, primitiveEntityRepresentation!);
                return new PrimitiveOrList { Primitive = new PrimitiveEntity { Decimal = validated.Decimal } };
            case "boolean":
                if (data.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                    throw new ParamValidationException("Cannot parse list " + data.List + " to primitive type");

                validated = ValidateBoolean(spec, primitiveEntityRepresentation!);
                return new PrimitiveOrList { Primitive = new PrimitiveEntity { Boolean = validated.Boolean } };
            case "array":
                if (data.ContentCase == PrimitiveOrList.ContentOneofCase.Primitive)
                {
                    var parsed = ValidatePrimitiveOrList(spec.Items, data);
                    if (parsed.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                    {
                        throw new ApiConfigException("Header spec contains nested lists! This is invalid.");
                    }
                    return new PrimitiveOrList
                    {
                        List =
                        {
                            Value = { new[] {parsed.Primitive} }
                        }
                    };
                }
                else
                {
                    var listEntity = new PrimitiveList();
                    foreach (var primitiveEntity in data.List.Value)
                    {
                        var parsed = ValidatePrimitiveOrList(spec.Items,
                            new PrimitiveOrList { Primitive = primitiveEntity });
                        if (parsed.ContentCase != PrimitiveOrList.ContentOneofCase.Primitive)
                        {
                            throw new ApiConfigException("Param spec contains nested lists! This is invalid.");
                        }
                        listEntity.Value.Add(parsed.Primitive);
                    }

                    return new PrimitiveOrList{ List = listEntity };
                }
            default:
                throw new ApiConfigException("Unsupported schema type " + spec.Type +
                                             ", expected primitive or list of primitives");
        }
    }

    private PrimitiveEntity ValidatePrimitive(OpenApiSchema spec, PrimitiveEntity data)
    {
        var primitiveEntityRepresentation = data.ContentCase switch
        {
            PrimitiveEntity.ContentOneofCase.Boolean => new Entity { Boolean = data.Boolean },
            PrimitiveEntity.ContentOneofCase.Decimal => new Entity { Decimal = data.Decimal },
            PrimitiveEntity.ContentOneofCase.Integer => new Entity { Integer = data.Integer },
            PrimitiveEntity.ContentOneofCase.String => new Entity { String = data.String },
            _ => throw new ApiRuntimeException("Unexpected primitive content case " + data.ContentCase)
        };
        
        switch (spec.Type)
        {
            case "string":
                var validated = ValidateString(spec, primitiveEntityRepresentation);
                return new PrimitiveEntity { String = validated.String };
            case "integer":
                validated = ValidateInteger(spec, primitiveEntityRepresentation);
                return new PrimitiveEntity { Integer = validated.Integer };
            case "number":
                validated = ValidateNumber(spec, primitiveEntityRepresentation);
                return new PrimitiveEntity { Decimal = validated.Decimal };
            case "boolean":
                validated = ValidateBoolean(spec, primitiveEntityRepresentation);
                return new PrimitiveEntity { Boolean = validated.Boolean };
            default:
                throw new ApiConfigException("Unsupported schema type " + spec.Type + ", expected primitive type");
        }
    }
}