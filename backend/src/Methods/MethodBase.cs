using System.Text.Json;
using Database.Json;

namespace Database.Methods;

public abstract class MethodBase<TInput, TOutput>
    : IMethod
{
    public JsonElement Calculate(JsonElement input)
    {
        var parsedInput = input.Deserialize<TInput>(JsonSerializerSettings.BedJson)
            ?? throw new JsonException($"Failed to deserialize the JSON input into a {typeof(TInput)} instance.");
        var output = Calculate(parsedInput);
        using var document = JsonDocument.Parse(
            JsonSerializer.SerializeToUtf8Bytes(output)
        );
        return document.RootElement.Clone();
    }

    public abstract TOutput Calculate(TInput input);
}