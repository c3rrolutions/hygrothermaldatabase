using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extractors;

namespace Database.Data;

public sealed class CalorimetricData
    : DataX
{
    public const string TableName = "calorimetric_data";

    public static readonly DataKind TheKind = DataKind.CALORIMETRIC_DATA;

    public new static readonly string AssertExistenceOfRootResourceTriggerName =
        DataX.AssertExistenceOfRootResourceTriggerName(TheKind);
    public new static readonly string CreateDataAccessPolicyIfNecessaryTriggerName =
        DataX.CreateDataAccessPolicyIfNecessaryTriggerName(TheKind);
    public static readonly ImmutableArray<string> TriggerNames = [
        AssertExistenceOfRootResourceTriggerName,
        CreateDataAccessPolicyIfNecessaryTriggerName
    ];

    public CalorimetricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
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
        DateTimeOffset createdAt,
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

    [NotMapped]
    public override DataKind Kind => TheKind;

    [InverseProperty(nameof(GetHttpsResource.CalorimetricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    [InverseProperty(nameof(DataAccessPolicy.CalorimetricData))]
    public override DataAccessPolicy? AccessPolicy { get; set; }

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