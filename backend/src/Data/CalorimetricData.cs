using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Extractors;

namespace Database.Data;

public sealed class CalorimetricData
    : DataX
{
    public CalorimetricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
        AppliedMethod appliedMethod,
        double[] gValues,
        double[] uValues
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
        GValues = gValues;
        UValues = uValues;
    }

    // `DbContext` needs this constructor without owned entities.
    public CalorimetricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
        double[] gValues,
        double[] uValues
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
        GValues = gValues;
        UValues = uValues;
    }

    [InverseProperty(nameof(GetHttpsResource.CalorimetricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    public double[] GValues { get; private set; }
    public double[] UValues { get; private set; }

    public override async Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    )
    {
        if (dataFormatId == IData.BedJsonDataFormatId)
        {
            GValues = await new DoubleResultsCalorimetricDataJsonExtractor(
                CalorimetricResult.G_VALUE
            ).Extract(filePath);
            UValues = await new DoubleResultsCalorimetricDataJsonExtractor(
                CalorimetricResult.U_VALUE
            ).Extract(filePath);
        }
    }
}