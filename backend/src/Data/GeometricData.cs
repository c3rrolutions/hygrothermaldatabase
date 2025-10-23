using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data;

public sealed class GeometricData
    : DataX
{
    public GeometricData(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod,
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
        Thicknesses = thicknesses;
    }

    public GeometricData(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
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
        Thicknesses = thicknesses;
    }

    [InverseProperty(nameof(GetHttpsResource.GeometricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    public double[] Thicknesses { get; private set; }
}