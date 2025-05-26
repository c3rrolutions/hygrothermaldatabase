using System;
using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    OpticalComponentType? Type,
    OpticalComponentSubtype? Subtype,
    CoatedSide? CoatedSide,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] NearnormalHemisphericalVisibleTransmittances,
    double[] NearnormalHemisphericalVisibleReflectances,
    double[] NearnormalHemisphericalSolarTransmittances,
    double[] NearnormalHemisphericalSolarReflectances,
    double[] InfraredEmittances,
    double[] ColorRenderingIndices,
    IReadOnlyList<CielabColorInput> CielabColors
);