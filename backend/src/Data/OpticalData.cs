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
    public OpticalData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
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
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
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

    public override async Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    )
    {
        if (dataFormatId == IData.BedJsonDataFormatId)
        {
            NearnormalHemisphericalVisibleTransmittances =
                await new DoubleResultsOpticalDataJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.VISIBLE,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.TRANSMITTANCE
                ).Extract(filePath);
            NearnormalHemisphericalVisibleReflectances =
                await new DoubleResultsOpticalDataJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.VISIBLE,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.REFLECTANCE
                ).Extract(filePath);
            NearnormalHemisphericalSolarTransmittances =
                await new DoubleResultsOpticalDataJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.SOLAR,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.TRANSMITTANCE
                ).Extract(filePath);
            NearnormalHemisphericalSolarReflectances =
                await new DoubleResultsOpticalDataJsonExtractor(
                    IncidenceDirection.NEARNORMAL,
                    WavelengthsIntegral.SOLAR,
                    EmergenceDirection.HEMISPHERICAL,
                    DataPointResult.REFLECTANCE
                ).Extract(filePath);
            InfraredEmittances =
                await new DoubleResultsOpticalDataJsonExtractor(
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