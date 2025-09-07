using System.Collections.Generic;
using System.Text.Json;

namespace Database.GraphQl.Methods;

public sealed class CalculateMethodPayload
    : Payload
{
    public CalculateMethodPayload(
        JsonDocument result
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
        JsonDocument result,
        IReadOnlyCollection<CalculateMethodError> errors
    )
    {
        Result = result;
        Errors = errors;
    }

    public CalculateMethodPayload(
        JsonDocument result,
        CalculateMethodError error
    )
        : this(
            result,
            [error]
        )
    {
    }

    public JsonDocument? Result { get; }
    public IReadOnlyCollection<CalculateMethodError>? Errors { get; }
}