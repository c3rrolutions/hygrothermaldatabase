using System;
using System.Collections.Generic;
using System.Linq;
using Database.Data;

namespace Database.GraphQl;

public sealed record AppliedMethodInput(
    Guid MethodId,
    IReadOnlyList<NamedMethodArgumentInput> Arguments,
    IReadOnlyList<NamedMethodSourceInput> Sources
)
{
    public AppliedMethod ToDomainModel()
    {
        return new AppliedMethod(
            MethodId,
            Arguments
                .Select(a => a.ToDomainModel())
                .ToList(),
            Sources
                .Select(s => s.ToDomainModel())
                .ToList()
        );
    }
};