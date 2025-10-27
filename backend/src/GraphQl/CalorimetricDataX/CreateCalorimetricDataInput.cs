using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;
using Database.Data;

namespace Database.GraphQl.CalorimetricDataX;

public sealed record CreateCalorimetricDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] GValues,
    double[] UValues
)
{
    public CalorimetricData ToDomainModel(Guid userId)
    {
        var calorimetricData = new CalorimetricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            GValues,
            UValues
        );
        calorimetricData.Resources.Add(RootResource.ToDomainModel());
        return calorimetricData;
    }
};