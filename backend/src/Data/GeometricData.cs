using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data;

public sealed class GeometricData
    : DataX
{
    public GeometricData(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        AppliedMethod appliedMethod,
        ICollection<DataApproval> approvals,
        // ResponseApproval approval,
        double[] thicknesses
    ) : base (
        locale,
        componentId,
        name,
        description,
        warnings,
        creatorId,
        createdAt,
        appliedMethod,
        approvals
        // approval
    )
    {
        Thicknesses = thicknesses;

    }
    public GeometricData(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
        // ResponseApproval approval,
        double[] thicknesses
    ) : base(
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
    public override ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();
    public double[] Thicknesses { get; private set;}

}