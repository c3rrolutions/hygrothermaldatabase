using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Extractors;
using NodaTime;

namespace Database.Data;

public sealed class GeometricData
    : DataX
{
    public GeometricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
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
        OffsetDateTime createdAt,
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

    [InverseProperty(nameof(GetHttpsResource.GeometricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

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