using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ByCoders.CNAB.Core.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string ToJson(this object obj, JsonSerializerOptions? options = default) =>
        JsonSerializer.Serialize(obj, options ?? _jsonSerializerOptions);

    public static TValue? FromJson<TValue>(this Stream json, JsonSerializerOptions? options = default) =>
        JsonSerializer.Deserialize<TValue>(json, options ?? _jsonSerializerOptions);
}