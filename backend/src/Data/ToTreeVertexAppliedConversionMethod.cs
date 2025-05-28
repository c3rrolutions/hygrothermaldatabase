using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class ToTreeVertexAppliedConversionMethod(
    Guid methodId,
    string sourceName
    )
{
    public ToTreeVertexAppliedConversionMethod(
        Guid methodId,
        ICollection<NamedMethodArgument> arguments,
        string sourceName
    )
        : this(
            methodId,
            sourceName
        )
    {
        Arguments = arguments;
    }

    public Guid MethodId { get; private set; } = methodId;
    public ICollection<NamedMethodArgument> Arguments { get; private set; } = [];
    public string SourceName { get; private set; } = sourceName;
}