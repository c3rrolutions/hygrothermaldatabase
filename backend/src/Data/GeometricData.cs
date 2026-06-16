using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extractors;

namespace Database.Data;

public sealed class GeometricData
    : DataX
{
    public const string TableName = "geometric_data";

    public static readonly DataKind TheKind = DataKind.GEOMETRIC_DATA;

    public new static readonly string AssertExistenceOfRootResourceTriggerName =
        DataX.AssertExistenceOfRootResourceTriggerName(TheKind);
    public new static readonly string CreateDataAccessPolicyIfNecessaryTriggerName =
        DataX.CreateDataAccessPolicyIfNecessaryTriggerName(TheKind);
    public static readonly ImmutableArray<string> TriggerNames = [
        AssertExistenceOfRootResourceTriggerName,
        CreateDataAccessPolicyIfNecessaryTriggerName
    ];

    public GeometricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
        AppliedMethod appliedMethod,
        double[] widths,
        double[] heights,
        double[] thicknesses
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
        Widths = widths;
        Heights = heights;
        Thicknesses = thicknesses;
    }

    public GeometricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
        double[] widths,
        double[] heights,
        double[] thicknesses
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
        Widths = widths;
        Heights = heights;
        Thicknesses = thicknesses;
    }

    [NotMapped]
    public override DataKind Kind => TheKind;

    [InverseProperty(nameof(GetHttpsResource.GeometricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    [InverseProperty(nameof(DataAccessPolicy.GeometricData))]
    public override DataAccessPolicy? AccessPolicy { get; set; }

    public double[] Widths { get; private set; }
    public double[] Heights { get; private set; }
    public double[] Thicknesses { get; private set; }

    public override async Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    )
    {
        if (dataFormatId == IData.BedJsonDataFormatId)
        {
            Widths = await new InstalledDimensionsGeometricDataJsonExtractor(
                Dimension.WIDTH
            ).Extract(filePath);
            Heights = await new InstalledDimensionsGeometricDataJsonExtractor(
                Dimension.HEIGHT
            ).Extract(filePath);
            Thicknesses = await new InstalledDimensionsGeometricDataJsonExtractor(
                Dimension.THICKNESS
            ).Extract(filePath);
        }
    }
}