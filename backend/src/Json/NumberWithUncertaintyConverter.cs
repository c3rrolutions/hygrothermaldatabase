using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Database.Json;

// Inspired by https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to
public sealed class NumberWithUncertaintyConverter
 : JsonConverter<double>
{
    private sealed record NumberWithUncertainty(
        double UncertainValue,
        [Range(0, double.MaxValue)]
        double Uncertainty,
        [AllowedValues(68.3, 95.4, 99.7, 100)]
        double ConfidenceInterval
    );

    public NumberWithUncertaintyConverter()
    {
    }

    private static JsonConverter<NumberWithUncertainty> GetNumberWithUncertaintyConverter(JsonSerializerOptions options)
        => (JsonConverter<NumberWithUncertainty>)options.GetConverter(typeof(NumberWithUncertainty));

    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.StartObject =>
                GetNumberWithUncertaintyConverter(options)
                .Read(ref reader, typeof(NumberWithUncertainty), options)
                ?.UncertainValue
                ?? throw new JsonException(),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}