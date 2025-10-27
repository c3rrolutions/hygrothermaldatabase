using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;
using Database.Data;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] Thicknesses
)
{
    public GeometricData ToDomainModel(Guid userId)
    {
        var geometricData = new GeometricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            Thicknesses
        );
        geometricData.Resources.Add(RootResource.ToDomainModel());
        return geometricData;
    }
};