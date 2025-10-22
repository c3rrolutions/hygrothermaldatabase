using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Database.Methods;

public abstract class MethodBase<TInput, TOutput>
    : IMethod
{
    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = false,
        IncludeFields = false,
        NumberHandling = JsonNumberHandling.Strict,
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
    };

    public JsonElement Calculate(JsonElement input)
    {
        var parsedInput = input.Deserialize<TInput>(JsonSerializerOptions)
            ?? throw new InvalidOperationException();
        var output = Calculate(parsedInput);
        using var document = JsonDocument.Parse(
            JsonSerializer.SerializeToUtf8Bytes(output)
        );
        return document.RootElement.Clone();
    }

    public abstract TOutput Calculate(TInput input);
}