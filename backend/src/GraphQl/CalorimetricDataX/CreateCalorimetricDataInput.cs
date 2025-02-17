using System;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;

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
    DataType? Type,
    DataSubtype? Subtype,
    CoatedSide? CoatedSide,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] GValues,
    double[] UValues
);