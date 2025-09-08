using System.Collections.Generic;
using System.Text.Json;

namespace Database.GraphQl.Methods;

public sealed class CalculateMethodPayload
    : Payload
{
    public CalculateMethodPayload(
        JsonElement result
    )
    {
        Result = result;
    }

    public CalculateMethodPayload(
        IReadOnlyCollection<CalculateMethodError> errors
    )
    {
        Errors = errors;
    }

    public CalculateMethodPayload(
        CalculateMethodError error
    )
        : this([error])
    {
    }

    public CalculateMethodPayload(
        JsonElement result,
        IReadOnlyCollection<CalculateMethodError> errors
    )
    {
        Result = result;
        Errors = errors;
    }

    public CalculateMethodPayload(
        JsonElement result,
        CalculateMethodError error
    )
        : this(
            result,
            [error]
        )
    {
    }

    public JsonElement? Result { get; }
    public IReadOnlyCollection<CalculateMethodError>? Errors { get; }
}