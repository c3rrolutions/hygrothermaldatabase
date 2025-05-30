using System;
using Database.Data;
using Database.GraphQl.Publications;
using Database.GraphQl.Standards;
using HotChocolate.Types;

namespace Database.GraphQl.References;

public sealed class ReferenceType
    : UnionType<IReference>
{
    internal static Reference FromInput(ReferenceInput input)
    {
        if (input?.Standard is null && input?.Publication is null)
        {
            throw new ArgumentException("Both the reference's standard and publication are null.", nameof(input));
        }
        if (input.Standard is not null && input.Publication is not null)
        {
            throw new ArgumentException("Both the reference's standard and publication are non-null.", nameof(input));
        }
        if (input.Standard is not null)
        {
            return new Reference(StandardType.FromInput(input.Standard));
        }
        else if (input.Publication is not null)
        {
            return new Reference(PublicationType.FromInput(input.Publication));
        }
        throw new ArgumentException("Impossible!", nameof(input));
    }

    protected override void Configure(IUnionTypeDescriptor descriptor)
    {
        descriptor.Name(nameof(IReference)[1..]);
    }
}