using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate;
using HotChocolate.Types;
using Database.Enumerations;
using Database.Data;

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
)
{
    public OpticalData ToDomainModel(Guid userId)
    {
        var opticalData = new OpticalData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            Type,
            Subtype,
            CoatedSide,
            AppliedMethod.ToDomainModel(),
            NearnormalHemisphericalVisibleTransmittances,
            NearnormalHemisphericalVisibleReflectances,
            NearnormalHemisphericalSolarTransmittances,
            NearnormalHemisphericalSolarReflectances,
            InfraredEmittances,
            ColorRenderingIndices,
            CielabColors.Select(c => c.ToDomainModel()).ToList()
        );
        opticalData.Resources.Add(RootResource.ToDomainModel());
        return opticalData;
    }
};