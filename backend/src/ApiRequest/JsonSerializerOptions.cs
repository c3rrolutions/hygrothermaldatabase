using System.Text.Json;
using System.Text.Json.Serialization;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Database.ApiRequest;

public static class JsonSerializerSettings
{
    public static readonly JsonSerializerOptions GraphQL =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();

    public static readonly JsonSerializerOptions REST =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();
}