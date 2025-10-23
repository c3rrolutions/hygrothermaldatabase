using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Enumerations.DataPoints;
using Database.Extractors;

namespace Database.Data;

public sealed class OpticalData
    : DataX
{
    public static readonly Guid BedJsonDataFormatId = new("9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708");

    public OpticalData(
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

    public async Task ExtractAndSetMirroredValuesFromFile(
        string filePath,
        Guid dataFormatId
    )
    {
        if (dataFormatId == BedJsonDataFormatId)
        {
            NearnormalHemisphericalVisibleTransmittances =
                await new DoubleResultsJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.VISIBLE,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.TRANSMITTANCE
                ).Extract(filePath);
            NearnormalHemisphericalVisibleReflectances =
                await new DoubleResultsJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.VISIBLE,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.REFLECTANCE
                ).Extract(filePath);
            NearnormalHemisphericalSolarTransmittances =
                await new DoubleResultsJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.SOLAR,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.TRANSMITTANCE
                ).Extract(filePath);
            NearnormalHemisphericalSolarReflectances =
                await new DoubleResultsJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.SOLAR,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.REFLECTANCE
                ).Extract(filePath);
            InfraredEmittances =
                await new DoubleResultsJsonExtractor(
                    IncidenceDirection.HEMISPHERICAL,
                    WavelengthsIntegral.INFRARED,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.ABSORPTANCE_EMITTANCE
                ).Extract(filePath);
            ColorRenderingIndices =
                await new ColorRenderingIndexResultsJsonExtractor()
                .Extract(filePath);
            CielabColors =
                await new CielabColorResultsJsonExtractor()
                .Extract(filePath);
        }
    }
}