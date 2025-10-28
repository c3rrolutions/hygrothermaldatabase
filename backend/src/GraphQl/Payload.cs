using System.Collections.Generic;

namespace Database.GraphQl;

public abstract class Payload
{
}

public sealed record Payload<TError>(
    IReadOnlyCollection<TError> Errors
);