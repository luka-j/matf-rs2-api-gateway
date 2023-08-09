using System.Globalization;
using System.Text;
using ApiGatewayApi;
using ApiGatewayRequestProcessor.Exceptions;
using Google.Protobuf.Collections;
using Serilog;

namespace ApiGatewayRequestProcessor.Utils;

public static class EntityUtils
{
    public static PrimitiveOrListObjectEntity ConvertToPrimitiveOrListObjectEntity(this ObjectEntity e)
    {
        var result = new PrimitiveOrListObjectEntity();
        foreach (var (key, value) in e.Properties)
        {
            var pol = new PrimitiveOrList();
            switch (value.ContentCase)
            {
                case Entity.ContentOneofCase.Boolean:
                case Entity.ContentOneofCase.Decimal:
                case Entity.ContentOneofCase.Integer:
                case Entity.ContentOneofCase.String:
                    pol.Primitive = value.ConvertToPrimitive();
                    break;
                case Entity.ContentOneofCase.List:
                    var primitivesList = value.List.Value.Select(ConvertToPrimitive);
                    var rf = new RepeatedField<PrimitiveEntity>();
                    foreach (var pe in primitivesList)
                    {
                        rf.Add(pe);
                    }

                    pol.List = new PrimitiveList
                    {
                        Value = { rf }
                    };
                    break;
                default:
                    throw new ApiRuntimeException("Invalid type for PrimitiveOrListObjectEntity: field " + key +
                                                  " is of type " + value.ContentCase);
            }
            result.Properties.Add(key, pol);
        }

        return result;
    }
    
    public static PrimitiveObjectEntity ConvertToPrimitiveObjectEntity(this ObjectEntity e)
    {
        var result = new PrimitiveObjectEntity();
        foreach (var (key, value) in e.Properties)
        {
            result.Properties.Add(key, value.ConvertToPrimitive());
        }

        return result;
    }

    private static PrimitiveEntity ConvertToPrimitive(this Entity value)
    {
        return value.ContentCase switch
        {
            Entity.ContentOneofCase.Boolean => new PrimitiveEntity { Boolean = value.Boolean },
            Entity.ContentOneofCase.Decimal => new PrimitiveEntity { Decimal = value.Decimal },
            Entity.ContentOneofCase.Integer => new PrimitiveEntity { Integer = value.Integer },
            Entity.ContentOneofCase.String => new PrimitiveEntity { String = value.String },
            _ => throw new ApiRuntimeException("Invalid type for primitive entity: " + value.ContentCase +
                                               " (while unpacking response)")
        };
    }

    public static Entity ConvertToObject(this PrimitiveOrListObjectEntity e)
    {
        var res = new ObjectEntity();
        foreach (var (key, value) in e.Properties)
        {
            var field = new Entity();
            switch (value.ContentCase)
            {
                case PrimitiveOrList.ContentOneofCase.List:
                    var list = value.List.Value.Select(ConvertToEntity);
                    var rf = new RepeatedField<Entity>();
                    foreach (var entity in list)
                    {
                        rf.Add(entity);
                    }
                    field.List = new ListEntity
                    {
                        Value = { rf }
                    };
                    break;
                case PrimitiveOrList.ContentOneofCase.Primitive:
                    field = value.Primitive.ConvertToEntity();
                    break;
                default:
                    throw new ApiRuntimeException("Invalid type in PrimitiveOrListObjectEntity: " + key + " is "
                                                  + value.ContentCase);
            }

            res.Properties.Add(key, field);
        }

        return new Entity
        {
            Object = res
        };
    }

    public static Entity ConvertToObject(this PrimitiveObjectEntity e)
    {
        var res = new ObjectEntity();
        foreach (var (key, value) in e.Properties)
        {
            res.Properties.Add(key, value.ConvertToEntity());
        }
        
        return new Entity
        {
            Object = res
        };
    }
    
    private static Entity ConvertToEntity(this PrimitiveEntity e)
    {
        return e.ContentCase switch
        {
            PrimitiveEntity.ContentOneofCase.Boolean => new Entity { Boolean = e.Boolean },
            PrimitiveEntity.ContentOneofCase.Decimal => new Entity { Decimal = e.Decimal },
            PrimitiveEntity.ContentOneofCase.Integer => new Entity { Integer = e.Integer },
            PrimitiveEntity.ContentOneofCase.String => new Entity { String = e.String },
            _ => throw new ApiRuntimeException("Invalid type for PrimitiveEntity: " + e.ContentCase)
        };
    }

    public static string AsString(this Entity entity)
    {
        return entity.ContentCase switch
        {
            Entity.ContentOneofCase.String => entity.String,
            Entity.ContentOneofCase.Boolean => entity.Boolean.ToString(),
            Entity.ContentOneofCase.Decimal => entity.Decimal.ToDecimal().ToString(CultureInfo.InvariantCulture),
            Entity.ContentOneofCase.Integer => entity.Integer.ToString(),
            Entity.ContentOneofCase.List => "[" + entity.List.Value.Select(c => c.AsString() + ",") + "]",
            Entity.ContentOneofCase.Object => entity.Object.Properties.Select(e => e.Key + "=>" + e.Value.AsString()).ToString()!,
            Entity.ContentOneofCase.None => "",
            _ => ""
        };
    }

    public static Entity? Find(this ObjectEntity entity, string target)
    {
        if (!target.StartsWith("${") && !target.EndsWith("}"))
        {
            throw new ApiRuntimeException("Invalid target to search for: " + target);
        }

        var path = target[2..^1].Split(".");
        var current = new Entity { Object = entity };
        foreach (var el in path)
        {
            if (current.ContentCase != Entity.ContentOneofCase.Object)
            {
                return null;
            }

            var obj = current.Object;

            if (el.Contains('[') && el.Contains(']'))
            {
                var fieldName = el[..el.IndexOf('[')];
                if (!int.TryParse(el[(el.IndexOf('[') + 1) .. el.IndexOf(']')], out var index))
                {
                    Log.Warning("Invalid index {Index} in expression {FieldName}", index, fieldName);
                }

                if (!obj.Properties.ContainsKey(fieldName))
                {
                    Log.Warning("Object doesn't contain field {FieldName}. Returning null from find",
                        fieldName);
                    return null;
                }

                var child = obj.Properties[fieldName];
                if (child.ContentCase != Entity.ContentOneofCase.List)
                {
                    Log.Warning("Expected field {Field} to be a list, got {ContentCase} instead",
                        fieldName, child.ContentCase);
                    return null;
                }

                if (child.List.Value.Count <= index)
                {
                    return null;
                }

                current = child.List.Value[index];
            }
            else
            {
                if (!obj.Properties.ContainsKey(el))
                {
                    Log.Warning("Object doesn't contain field {FieldName}. Returning null from find", el);
                    return null;
                }

                current = obj.Properties[el];
            }
        }

        return current;
    }

    public static void Delete(this ObjectEntity entity, string target)
    {
        if (target.StartsWith("${") && target.EndsWith("}"))
        {
            target = target[2..^1];
        }

        var fullPath = target.Split(".");
        var path = fullPath[..^1];
        
        var current = new Entity { Object = entity };
        foreach (var el in path)
        {
            if (current.ContentCase != Entity.ContentOneofCase.Object)
            {
                return;
            }

            var obj = current.Object;

            if (el.Contains('[') && el.Contains(']'))
            {
                var fieldName = el[..el.IndexOf('[')];
                if (!int.TryParse(el[(el.IndexOf('[') + 1) .. el.IndexOf(']')], out var index))
                {
                    Log.Warning("Invalid index {Index} in expression {FieldName}", index, fieldName);
                }

                if (!obj.Properties.ContainsKey(fieldName))
                {
                    Log.Warning("Object doesn't contain field {FieldName}, not deleting anything",
                        fieldName);
                    return;
                }

                var child = obj.Properties[fieldName];
                if (child.ContentCase != Entity.ContentOneofCase.List)
                {
                    Log.Warning("Expected field {Field} to be a list, got {ContentCase} instead, not " +
                                "deleting anything", fieldName, child.ContentCase);
                    return;
                }

                if (child.List.Value.Count <= index)
                {
                    return;
                }

                current = child.List.Value[index];
            }
            else
            {
                if (!obj.Properties.ContainsKey(el))
                {
                    Log.Warning("Object doesn't contain field {FieldName}, not deleting anything", el);
                    return;
                }

                current = obj.Properties[el];
            }
        }

        var destination = fullPath.Last();
        var lastObj= current.Object;
        if (destination.Contains('[') && destination.Contains(']'))
        {
            var fieldName = destination[..destination.IndexOf('[')];
            var indexStr = destination[(destination.IndexOf('[') + 1) .. destination.IndexOf(']')];
            if (!int.TryParse(indexStr, out var index))
            {
                Log.Warning("Invalid index {Index} in expression {FieldName}", index, fieldName);
                throw new ApiRuntimeException("Index is not a valid number (" + target + ")!");
            }

            if (!lastObj.Properties.ContainsKey(fieldName))
            {
                Log.Warning("Attempted to delete entity (list) that doesn't exist: {@Target}",
                    target);
                return;
            }
            
            var destObj = lastObj.Properties[fieldName];
            if (destObj.ContentCase != Entity.ContentOneofCase.List)
            {
                Log.Warning("Attempted to delete {@Target} in {@Entity}, " +
                            "but location isn't a list!", target, entity);
                return;
            }

            var destList = destObj.List;
            if (destList.Value.Count <= index)
            {
                Log.Warning("Attempted to delete entity {@Target}, but list is shorter than location",
                    target);
                return;
            }
            
            destList.Value.RemoveAt(index);
        }
        else
        {
            lastObj.Properties.Remove(destination);
        }
    }

    public static void Insert(this ObjectEntity parent, Entity entity, string target)
    {
        if (target.StartsWith("${") && target.EndsWith("}"))
        {
            target = target[2..^1];
        }

        var fullPath = target.Split(".");
        var path = fullPath[..^1];

        var current = new Entity { Object = parent };
        foreach (var el in path)
        {
            if (current.ContentCase != Entity.ContentOneofCase.Object)
            {
                Log.Warning("Attempted to insert entity {@Entity} to parent {@Parent}, but location " +
                            "${Location} is invalid", entity, parent, target);
                throw new ApiRuntimeException("Cannot insert to non-object entity!");
            }

            var obj = current.Object;

            if (el.Contains('[') && el.Contains(']'))
            {
                var fieldName = el[..el.IndexOf('[')];
                var indexStr = el[(el.IndexOf('[') + 1) .. el.IndexOf(']')];
                
                if (!obj.Properties.ContainsKey(fieldName))
                {
                    current = new Entity{ Object = new ObjectEntity() };
                    var newList = new Entity { List = new ListEntity { Value = { current }} };
                    obj.Properties.Add(fieldName, newList);
                    continue;
                }

                if (!int.TryParse(indexStr, out var index))
                {
                    Log.Warning("Invalid index {Index} in expression {FieldName}", index, fieldName);
                }

                var child = obj.Properties[fieldName];
                if (child.ContentCase != Entity.ContentOneofCase.List)
                {
                    Log.Warning("Expected field {Field} in object {@Parent} (location {@Location}) to be " +
                                "a list, got {ContentCase} instead",
                        fieldName, parent, target, child.ContentCase);
                    throw new ApiRuntimeException("Cannot insert to non-object entity!");
                }

                current = child.List.Value[index];
            }
            else
            {
                if (!obj.Properties.ContainsKey(el))
                {
                    current = new Entity { Object = new ObjectEntity() };
                    obj.Properties.Add(el, current);
                    continue;
                }

                current = obj.Properties[el];
            }
        }

        var destination = fullPath.Last();
        var lastObj= current.Object;
        if (destination.Contains('[') && destination.Contains(']'))
        {
            var fieldName = destination[..destination.IndexOf('[')];
            var indexStr = destination[(destination.IndexOf('[') + 1) .. destination.IndexOf(']')];

            if (!lastObj.Properties.ContainsKey(fieldName))
            {
                lastObj.Properties[fieldName] = new Entity { List = new ListEntity { Value = { entity } } };
                return;
            }

            var destObj = lastObj.Properties[fieldName];
            if (destObj.ContentCase != Entity.ContentOneofCase.List)
            {
                Log.Warning("Attempted to insert {@Entity} to {@Parent} on location {@Location}, " +
                            "but location isn't a list!", entity, parent, target);
                throw new ApiRuntimeException("Cannot insert to an indexed location of non-indexable entity");
            }

            if (indexStr == "*")
            {
                destObj.List.Value.Add(entity);
            }
            else
            {
                if (!int.TryParse(indexStr, out var index))
                {
                    Log.Warning("Invalid index {Index} in expression {FieldName}", index, fieldName);
                    throw new ApiRuntimeException("Index is not a valid number (" + target + ")!");
                }

                if (destObj.List.Value.Count <= index)
                {
                    Log.Warning("Attempted to insert {@Entity} to {@Parent} on location {@Location}, " +
                                "but location is out of bounds!", entity, parent, target);
                    throw new ApiRuntimeException("Index is out of bounds (" + target + ")!");
                }

                destObj.List.Value[index] = entity;
            }
        }
        else
        {
            if (lastObj == null)
            {
                lastObj = new ObjectEntity();
            }
            lastObj.Properties[destination] = entity;
        }
    }

    public static string Substitute(this ObjectEntity entity, string template)
    {
        var inExpression = false;
        var expression = new StringBuilder();
        var result = new StringBuilder();
        for (var i = 0; i < template.Length; i++)
        {
            var ch = template[i];
            if (ch == '$' && template.Length > i + 2 && template[i + 1] == '{') // start ${ ... } expression
            {
                inExpression = true;
                expression.Append("${");
                i++;
                continue;
            }
            
            if (inExpression)
            {
                expression.Append(ch);
            }

            if (inExpression && ch == '}')
            {
                var substituted = entity.Find(expression.ToString());
                if (substituted == null)
                {
                    Log.Warning("Not found entity at {Expression} for template {Template}, ignoring", 
                        expression, template);
                }
                else
                {
                    result.Append(substituted.AsString());
                }

                inExpression = false;
                expression.Clear();
            } else if (!inExpression)
            {
                result.Append(ch);
            }
        }

        return result.ToString();
    }
}