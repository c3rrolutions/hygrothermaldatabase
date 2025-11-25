using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Database.Data;

public sealed class PhotovoltaicData
    : DataX
{
    public PhotovoltaicData(
        Guid? userId,
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
    public PhotovoltaicData(
        Guid? userId,
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

    [InverseProperty(nameof(GetHttpsResource.PhotovoltaicData))]
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