using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;

namespace Database.Data;

public sealed class OpticalData
    : DataX
{
    public OpticalData(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        OpticalComponentType? type,
        OpticalComponentSubtype? subtype,
        CoatedSide? coatedSide,
        AppliedMethod appliedMethod,
        double[] nearnormalHemisphericalVisibleTransmittances,
        double[] nearnormalHemisphericalVisibleReflectances,
        double[] nearnormalHemisphericalSolarTransmittances,
        double[] nearnormalHemisphericalSolarReflectances,
        double[] infraredEmittances,
        double[] colorRenderingIndices,
        ICollection<CielabColor> cielabColors
    ) : base(
        userId,
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        appliedMethod
    )
    {
        Type = type;
        Subtype = subtype;
        CoatedSide = coatedSide;
        NearnormalHemisphericalVisibleTransmittances = nearnormalHemisphericalVisibleTransmittances;
        NearnormalHemisphericalVisibleReflectances = nearnormalHemisphericalVisibleReflectances;
        NearnormalHemisphericalSolarTransmittances = nearnormalHemisphericalSolarTransmittances;
        NearnormalHemisphericalSolarReflectances = nearnormalHemisphericalSolarReflectances;
        InfraredEmittances = infraredEmittances;
        ColorRenderingIndices = colorRenderingIndices;
        CielabColors = cielabColors;
    }

    // `DbContext` needs this constructor without owned entities.
    public OpticalData(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        OpticalComponentType? type,
        OpticalComponentSubtype? subtype,
        CoatedSide? coatedSide,
        double[] nearnormalHemisphericalVisibleTransmittances,
        double[] nearnormalHemisphericalVisibleReflectances,
        double[] nearnormalHemisphericalSolarTransmittances,
        double[] nearnormalHemisphericalSolarReflectances,
        double[] infraredEmittances,
        double[] colorRenderingIndices
    ) : base(
        userId,
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt
    )
    {
        Type = type;
        Subtype = subtype;
        CoatedSide = coatedSide;
        NearnormalHemisphericalVisibleTransmittances = nearnormalHemisphericalVisibleTransmittances;
        NearnormalHemisphericalVisibleReflectances = nearnormalHemisphericalVisibleReflectances;
        NearnormalHemisphericalSolarTransmittances = nearnormalHemisphericalSolarTransmittances;
        NearnormalHemisphericalSolarReflectances = nearnormalHemisphericalSolarReflectances;
        InfraredEmittances = infraredEmittances;
        ColorRenderingIndices = colorRenderingIndices;
    }

    [InverseProperty(nameof(GetHttpsResource.OpticalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    public OpticalComponentType? Type { get; private set; }
    public OpticalComponentSubtype? Subtype { get; private set; }
    public CoatedSide? CoatedSide { get; private set; }
    public double[] NearnormalHemisphericalVisibleTransmittances { get; private set; }
    public double[] NearnormalHemisphericalVisibleReflectances { get; private set; }
    public double[] NearnormalHemisphericalSolarTransmittances { get; private set; }
    public double[] NearnormalHemisphericalSolarReflectances { get; private set; }
    public double[] InfraredEmittances { get; private set; }
    public double[] ColorRenderingIndices { get; private set; }
    public ICollection<CielabColor> CielabColors { get; private set; } = [];
}