using System;
using Database.Data;
using Database.GraphQl.Publications;
using Database.GraphQl.Standards;
using HotChocolate.Types;

namespace Database.GraphQl.References;

[OneOf]
public sealed record ReferenceInput(
    StandardInput? Standard,
    PublicationInput? Publication
)
{
    public Reference ToDomainModel()
    {
        if (Standard is null && Publication is null)
        {
            throw new InvalidOperationException("Both the reference's standard and publication are null.");
        }
        if (Standard is not null && Publication is not null)
        {
            throw new InvalidOperationException("Both the reference's standard and publication are non-null.");
        }
        if (Standard is not null)
        {
            return new Reference(Standard.ToDomainModel());
        }
        else if (Publication is not null)
        {
            return new Reference(Publication.ToDomainModel());
        }
        throw new InvalidOperationException("Impossible!");
    }
};