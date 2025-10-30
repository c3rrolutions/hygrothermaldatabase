using System.Collections.Generic;

namespace Database.GraphQl;

public abstract record Payload
{
}

public sealed record Payload<TError>(
    IReadOnlyCollection<TError> Errors
);