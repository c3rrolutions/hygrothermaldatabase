using System;
using System.Collections.Generic;
using System.Linq;
using Database.Data;

namespace Database.GraphQl;

public sealed record ToTreeVertexAppliedConversionMethodInput(
    Guid MethodId,
    IReadOnlyList<NamedMethodArgumentInput> Arguments,
    string SourceName
)
{
    public ToTreeVertexAppliedConversionMethod ToDomainModel()
    {
        return new ToTreeVertexAppliedConversionMethod(
            MethodId,
            Arguments.Select(a => a.ToDomainModel()).ToList(),
            SourceName
        );
    }
};