using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.GraphQl;

namespace Database.Data;

[JsonPolymorphic(TypeDiscriminatorPropertyName = GraphQlConstants.TypeDiscriminatorPropertyName)]
[JsonDerivedType(typeof(CalorimetricData), typeDiscriminator: nameof(CalorimetricData))]
[JsonDerivedType(typeof(GeometricData), typeDiscriminator: nameof(GeometricData))]
[JsonDerivedType(typeof(HygrothermalData), typeDiscriminator: nameof(HygrothermalData))]
[JsonDerivedType(typeof(LifeCycleData), typeDiscriminator: nameof(LifeCycleData))]
[JsonDerivedType(typeof(OpticalData), typeDiscriminator: nameof(OpticalData))]
[JsonDerivedType(typeof(PhotovoltaicData), typeDiscriminator: nameof(PhotovoltaicData))]
public interface IData : IEntity, IAuditable
{
    public static readonly Guid BedJsonDataFormatId = new("9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708");

    Guid? UserId { get; }
    Guid ComponentId { get; }
    string? Name { get; }
    string? Description { get; }
    string[] Warnings { get; }
    Guid CreatorId { get; }
    AppliedMethod AppliedMethod { get; }
    ICollection<DataApproval> Approvals { get; }
    ICollection<GetHttpsResource> Resources { get; }
    ResponseApproval? Approval { get; set; }
    string Locale { get; }
    DataAccessPolicy? AccessPolicy { get; set; }
    PublishingState PublishingState { get; }

    [NotMapped]
    DataKind Kind { get; }

    void Update(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        DateTimeOffset createdAt,
        Guid creatorId
    );

    void Publish();

    void Retract();

    Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    );
}