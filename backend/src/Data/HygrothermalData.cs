using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;

namespace Database.Data;

public sealed class HygrothermalData
    : DataX
{
    public HygrothermalData(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        DataType? type,
        DataSubtype? subtype,
        CoatedSide? coatedSide,
        AppliedMethod appliedMethod
    ) : base(
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        type,
        subtype,
        coatedSide,
        appliedMethod
    )
    {
    }

    // `DbContext` needs this constructor without owned entities.
    public HygrothermalData(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        DataType? type,
        DataSubtype? subtype,
        CoatedSide? coatedSide
    ) : base(
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        type,
        subtype,
        coatedSide
    )
    {
    }

    [InverseProperty(nameof(GetHttpsResource.HygrothermalData))]
    public override ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();
}