using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.GraphQl;

namespace Database.Data;

[JsonPolymorphic(TypeDiscriminatorPropertyName = GraphQlConstants.TypeDiscriminatorPropertyName)]
[JsonDerivedType(typeof(CalorimetricData), typeDiscriminator: nameof(CalorimetricData))]
[JsonDerivedType(typeof(GeometricData), typeDiscriminator: nameof(GeometricData))]
[JsonDerivedType(typeof(HygrothermalData), typeDiscriminator: nameof(HygrothermalData))]
[JsonDerivedType(typeof(OpticalData), typeDiscriminator: nameof(OpticalData))]
[JsonDerivedType(typeof(PhotovoltaicData), typeDiscriminator: nameof(PhotovoltaicData))]
public interface IData : IEntity
{
    public static readonly Guid BedJsonDataFormatId = new("9ca9e8f5-94bf-4fdd-81e3-31a58d7ca708");

    Guid? UserId { get; }
    Guid ComponentId { get; }
    string? Name { get; }
    string? Description { get; }
    string[] Warnings { get; }
    Guid CreatorId { get; }
    OffsetDateTime CreatedAt { get; }
    AppliedMethod AppliedMethod { get; }
    ICollection<DataApproval> Approvals { get; }
    ICollection<GetHttpsResource> Resources { get; }
    ResponseApproval? Approval { get; set; }
    string Locale { get; }
    DataAccessRights DataAccessRights { get; }
    PublishingState PublishingState { get; }

    /// <summary>
    /// Check if dataset is restricted for passed application id.
    /// </summary>
    /// <param name="applicationId"> Id of application. </param>
    /// <returns> True, if dataset is rescricted. Otherwise false. </returns>
    bool IsRestrictedByApplication(string applicationId);

    /// <summary>
    /// Check if dataset is restricted for at least one of the passed institutions.
    /// </summary>
    /// <param name="institutions"> List of institution ids. </param>
    /// <returns> True, if dataset is rescricted. Otherwise false. </returns>
    bool IsRestrictedByInstitutions(IEnumerable<Guid> institutions);

    /// <summary>
    /// Check if dataset is restricted for passed user.
    /// </summary>
    /// <param name="uuid">                Id of user. </param>
    /// <param name="alreadyAccesedCount"> Count of already accessed datasets by user. </param>
    /// <returns> True, if dataset is rescricted. Otherwise false. </returns>
    bool IsRestrictedByUser(Guid uuid, uint alreadyAccesedCount);

    void Update(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        OffsetDateTime createdAt,
        Guid creatorId
    );

    void Publish();

    void Retract();

    Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    );
}