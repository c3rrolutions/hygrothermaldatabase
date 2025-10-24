using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Database.ApiRequests;

/// <summary>
/// Options for JSON serialization.
/// </summary>
public static class JsonSerializerSettings
{
    private static readonly JsonSerializerOptions s_common =
        new()
        {
            AllowOutOfOrderMetadataProperties = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            ReadCommentHandling = JsonCommentHandling.Disallow,
        };

    public static readonly JsonSerializerOptions GraphQl =
        new(s_common)
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

    public static readonly JsonSerializerOptions Rest =
        new(s_common)
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

    public static readonly JsonSerializerOptions BedJson =
        new(s_common)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
}