using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;

namespace Database.Data;

public sealed class LifeCycleData
    : DataX
{
    public const string TableName = "lifeCycle_data";

    public LifeCycleData(
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
    public LifeCycleData(
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

    [InverseProperty(nameof(GetHttpsResource.LifeCycleData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    [InverseProperty(nameof(DataAccessPolicy.LifeCycleData))]
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