using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using Json.Path;
using Database.Extensions;
using System.Collections.Generic;

namespace Database.Extractors;

public class ClassJsonPathExtractor<TOutput>(
    JsonPath jsonPath,
    Func<JsonNode, TOutput?> parseNode
)
 : JsonExtractorBase<ICollection<TOutput>>
 where TOutput : class
{
    public override ICollection<TOutput> Extract(JsonDocument jsonDocument)
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
            .ToList();
    }

}