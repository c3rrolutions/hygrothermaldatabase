using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data;

public sealed class HygrothermalData
    : DataX
{
    public HygrothermalData(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
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
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt
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
}