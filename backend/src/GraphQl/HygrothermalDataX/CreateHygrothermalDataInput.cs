using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;
using Database.Data;

namespace Database.GraphQl.HygrothermalDataX;

public sealed record CreateHygrothermalDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource
)
{
    public HygrothermalData ToDomainModel(Guid userId)
    {
        var hygrothermalData = new HygrothermalData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel()
        );
        hygrothermalData.Resources.Add(RootResource.ToDomainModel());
        return hygrothermalData;
    }
};