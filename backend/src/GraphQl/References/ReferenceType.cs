using System;
using Database.Data;
using Database.GraphQl.Publications;
using Database.GraphQl.Standards;
using HotChocolate.Types;

namespace Database.GraphQl.References;

public sealed class ReferenceType
    : UnionType<IReference>
{
    protected override void Configure(IUnionTypeDescriptor descriptor)
    {
        descriptor.Name(nameof(IReference)[1..]);
    }
}