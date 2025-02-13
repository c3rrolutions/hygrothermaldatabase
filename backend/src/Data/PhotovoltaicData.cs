using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;

namespace Database.Data;

public sealed class PhotovoltaicData
    : DataX
{
    public PhotovoltaicData(
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
        ICollection<DataApproval> approvals
        // ResponseApproval approval
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
        appliedMethod,
        approvals
    )
    {
    }

    // `DbContext` needs this constructor without owned entities.
    public PhotovoltaicData(
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
        // ResponseApproval approval
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

    [InverseProperty(nameof(GetHttpsResource.PhotovoltaicData))]
    public override ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();
}