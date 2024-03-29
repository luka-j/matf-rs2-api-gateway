﻿using CCO.Entities;
using System.Text.Json;

namespace CCO.CCOConfigs
{
    public class CCOConfig<TSource> where TSource : IDatasource
    {
        public DateTime ValidFrom { get; }
        public CCOConfigIdentifier Id { get; }
        public Spec<TSource> Data { get; }

        public CCOConfig(DateTime validFrom, string jsonString)
        {
            ValidFrom = validFrom;
            Data = ParseJsonString(jsonString);
            Id = new CCOConfigIdentifier(Data.Title);
        }

        public bool IsActive(DateTime now)
        {
            return now >= ValidFrom;
        }

        public static Spec<TSource> ParseJsonString(string jsonString)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<Spec<TSource>>(jsonString, options) ?? throw new Exception("Exception during deserialization");
        }
        public string GetDataString()
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(Data, options);
        }
    }
}
