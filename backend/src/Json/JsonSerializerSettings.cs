using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Serializer.SystemTextJson;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Database.Json;

/// <summary>
/// Options for JSON serialization.
/// </summary>
public static class JsonSerializerSettings
{
    private static readonly JsonSerializerOptions s_common =
        new JsonSerializerOptions()
        {
            AllowOutOfOrderMetadataProperties = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = false,
            IncludeFields = false,
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            RespectNullableAnnotations = true,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            AllowDuplicateProperties = false,
        }
        .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    public static readonly JsonSerializerOptions GraphQl =
        new(s_common)
        {
            Converters = { new JsonStringEnumConverter(new ToConstantCaseJsonNamingPolicy(), allowIntegerValues: false) },
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
            Converters = { new JsonStringEnumConverter(new ToCamelCaseJsonNamingPolicy(), allowIntegerValues: false) },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
}