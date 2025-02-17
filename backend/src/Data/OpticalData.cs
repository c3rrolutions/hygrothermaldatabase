using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;

namespace Database.Data;

public sealed class OpticalData
    : DataX
{
    public OpticalData(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        DataType? type,
        DataSubtype? subtype,
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
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        type,
        subtype,
        coatedSide,
        appliedMethod
    )
    {
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
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        DataType? type,
        DataSubtype? subtype,
        CoatedSide? coatedSide,
        double[] nearnormalHemisphericalVisibleTransmittances,
        double[] nearnormalHemisphericalVisibleReflectances,
        double[] nearnormalHemisphericalSolarTransmittances,
        double[] nearnormalHemisphericalSolarReflectances,
        double[] infraredEmittances,
        double[] colorRenderingIndices
    ) : base(
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        type,
        subtype,
        coatedSide
    )
    {
        NearnormalHemisphericalVisibleTransmittances = nearnormalHemisphericalVisibleTransmittances;
        NearnormalHemisphericalVisibleReflectances = nearnormalHemisphericalVisibleReflectances;
        NearnormalHemisphericalSolarTransmittances = nearnormalHemisphericalSolarTransmittances;
        NearnormalHemisphericalSolarReflectances = nearnormalHemisphericalSolarReflectances;
        InfraredEmittances = infraredEmittances;
        ColorRenderingIndices = colorRenderingIndices;
    }

    [InverseProperty(nameof(GetHttpsResource.OpticalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();

    public double[] NearnormalHemisphericalVisibleTransmittances { get; private set; }
    public double[] NearnormalHemisphericalVisibleReflectances { get; private set; }
    public double[] NearnormalHemisphericalSolarTransmittances { get; private set; }
    public double[] NearnormalHemisphericalSolarReflectances { get; private set; }
    public double[] InfraredEmittances { get; private set; }
    public double[] ColorRenderingIndices { get; private set; }
    public ICollection<CielabColor> CielabColors { get; private set; } = new List<CielabColor>();
}