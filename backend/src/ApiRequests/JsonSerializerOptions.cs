using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Database.ApiRequests;

/// <summary>
/// Settings for Json serialization.
/// </summary>
public static class JsonSerializerSettings
{
    /// <summary>
    /// Settings for GraphQL Json requests.
    /// </summary>
    public static readonly JsonSerializerOptions GraphQL =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            AllowOutOfOrderMetadataProperties = true,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();

    /// <summary>
    /// Settings for REST Json requests.
    /// </summary>
    public static readonly JsonSerializerOptions Rest =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            AllowOutOfOrderMetadataProperties = true,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();
}