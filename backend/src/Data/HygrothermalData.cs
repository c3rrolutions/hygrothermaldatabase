using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;
using Database.Enumerations;

namespace Database.Data;

public sealed class HygrothermalData
    : DataX
{
    public const string TableName = "hygrothermal_data";

    public static readonly DataKind TheKind = DataKind.HYGROTHERMAL_DATA;

    public new static readonly string AssertExistenceOfRootResourceTriggerName =
        DataX.AssertExistenceOfRootResourceTriggerName(TheKind);
    public new static readonly string CreateDataAccessPolicyIfNecessaryTriggerName =
        DataX.CreateDataAccessPolicyIfNecessaryTriggerName(TheKind);
    public static readonly ImmutableArray<string> TriggerNames = [
        AssertExistenceOfRootResourceTriggerName,
        CreateDataAccessPolicyIfNecessaryTriggerName
    ];

    public HygrothermalData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
        AppliedMethod appliedMethod
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
    }

    // `DbContext` needs this constructor without owned entities.
    public HygrothermalData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt
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
    }

    [NotMapped]
    public override DataKind Kind => TheKind;

    [InverseProperty(nameof(GetHttpsResource.HygrothermalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    [InverseProperty(nameof(DataAccessPolicy.HygrothermalData))]
    public override DataAccessPolicy? AccessPolicy { get; set; }

    public override Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    )
    {
        // if (dataFormatId == IData.BedJsonDataFormatId)
        // {
        // }
        return Task.CompletedTask;
    }
}