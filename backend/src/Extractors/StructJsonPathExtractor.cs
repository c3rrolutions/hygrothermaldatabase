using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Database.Extensions;
using Json.More;
using Json.Path;

namespace Database.Extractors;

public class StructJsonPathExtractor<TOutput>(
    JsonPath jsonPath,
    Func<JsonNode, TOutput?> parseNode
)
 : JsonExtractorBase<TOutput[]>
 where TOutput : struct
{
    public override TOutput[] Extract(JsonDocument jsonDocument)
    {
        var pathResult = jsonPath.Evaluate(jsonDocument.RootElement.AsNode());
        // `JsonPath` defers evaluation according to
        // https://docs.json-everything.net/path/basics/#deferred-execution
        // Because the `JsonDocument` will have been disposed of more or less
        // after this method returns, we need to evaluate the enumerable
        // `pathResult.Matches` directly by calling `ToArray` instead of
        // deferring it until later.
        return pathResult.Matches
            .Where(node => node.Value is not null)
            .Select(node => parseNode(node.Value!)) // using `!` is safe here due to the `is not null` filter
            .NotNull()
            .ToArray();
    }

    protected static double? ExtractNumberWithUncertainty(JsonNode node)
    {
        return node.GetValueKind() switch
        {
            JsonValueKind.Object =>
                node.AsObject()
                    .TryGetPropertyValue("uncertainValue", out var uncertainValueNode)
                    ? (uncertainValueNode.TryGetValue<double>(out var uncertainValue) ? uncertainValue : null)
                    : null,
            JsonValueKind.Number => node.TryGetValue<double>(out var value) ? value : null,
            _ => null,
        };
    }
}