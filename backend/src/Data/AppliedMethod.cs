using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class AppliedMethod(
    Guid methodId
    )
{
    public AppliedMethod(
        Guid methodId,
        ICollection<NamedMethodArgument> arguments,
        ICollection<NamedMethodSource> sources
    )
        : this(methodId)
    {
        Arguments = arguments;
        Sources = sources;
    }

    public Guid MethodId { get; private set; } = methodId;
    public ICollection<NamedMethodArgument> Arguments { get; private set; } = new List<NamedMethodArgument>();
    public ICollection<NamedMethodSource> Sources { get; private set; } = new List<NamedMethodSource>();
}