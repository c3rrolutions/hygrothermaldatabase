using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Database.Data;

public sealed class HygrothermalData
    : DataX
{
    public HygrothermalData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        OffsetDateTime createdAt,
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
        OffsetDateTime createdAt
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

    [InverseProperty(nameof(GetHttpsResource.HygrothermalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

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