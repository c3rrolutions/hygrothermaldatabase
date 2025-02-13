using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;

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
        DataType? type,
        DataSubtype? subtype,
        CoatedSide? coatedSide,
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
        type,
        subtype,
        coatedSide,
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
        DataType? type,
        DataSubtype? subtype,
        CoatedSide? coatedSide,
        // ResponseApproval approval,
        double[] thicknesses
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
        Thicknesses = thicknesses;
    }

    [InverseProperty(nameof(GetHttpsResource.GeometricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();
    public double[] Thicknesses { get; private set;}

}