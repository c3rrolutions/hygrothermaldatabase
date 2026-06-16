using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Enumerations.DataPoints;
using Database.Extractors;
using Database.Data.AccessPolicies;

namespace Database.Data;

public sealed class OpticalData
    : DataX
{
    public const string TableName = "optical_data";

    public static readonly DataKind TheKind = DataKind.OPTICAL_DATA;

    public new static readonly string AssertExistenceOfRootResourceTriggerName =
        DataX.AssertExistenceOfRootResourceTriggerName(TheKind);
    public new static readonly string CreateDataAccessPolicyIfNecessaryTriggerName =
        DataX.CreateDataAccessPolicyIfNecessaryTriggerName(TheKind);
    public static readonly ImmutableArray<string> TriggerNames = [
        AssertExistenceOfRootResourceTriggerName,
        CreateDataAccessPolicyIfNecessaryTriggerName
    ];


    public OpticalData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
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
        DateTimeOffset createdAt,
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

    [NotMapped]
    public override DataKind Kind => TheKind;

    [InverseProperty(nameof(GetHttpsResource.OpticalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    [InverseProperty(nameof(DataAccessPolicy.OpticalData))]
    public override DataAccessPolicy? AccessPolicy { get; set; }

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