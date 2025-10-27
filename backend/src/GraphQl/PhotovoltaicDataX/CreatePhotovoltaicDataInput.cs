using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;
using Database.Data;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed record CreatePhotovoltaicDataInput(
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
    public PhotovoltaicData ToDomainModel(Guid userId)
    {
        var photovoltaicData = new PhotovoltaicData(
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
        photovoltaicData.Resources.Add(RootResource.ToDomainModel());
        return photovoltaicData;
    }
};