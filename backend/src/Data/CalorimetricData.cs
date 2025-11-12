using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Data;

public sealed class CalorimetricData
    : DataX
{
    public CalorimetricData(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
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
        DateTime createdAt,
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

    [InverseProperty(nameof(GetHttpsResource.CalorimetricData))]
    public override ICollection<GetHttpsResource> Resources { get; } = [];

    public double[] GValues { get; private set; }
    public double[] UValues { get; private set; }
}