using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Enumerations;

namespace Database.Data;

public abstract class DataX : AuditableEntity, IData
{
    public DataX(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt
    )
    {
        UserId = userId;
        Locale = locale;
        ComponentId = componentId;
        Name = name;
        Description = description;
        Warnings = warnings;
        CreatorId = creatorId;
        CreatedAt = createdAt;
    }

    protected DataX(
        Guid? userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTimeOffset createdAt,
        AppliedMethod appliedMethod
    )
        : this(
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
        AppliedMethod = appliedMethod;
    }

    public void Update(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        DateTimeOffset createdAt,
        Guid creatorId
    )
    {
        Locale = locale;
        ComponentId = componentId;
        Name = name;
        Description = description;
        Warnings = warnings;
        CreatedAt = createdAt;
        CreatorId = creatorId;
    }

    public void Publish()
    {
        PublishingState = PublishingState.PUBLISHED;
    }

    public void Retract()
    {
        PublishingState = PublishingState.RETRACTED;
    }

    public Guid? UserId { get; private set; }
    public string Locale { get; private set; }
    public Guid ComponentId { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string[] Warnings { get; private set; }
    public Guid CreatorId { get; private set; }
    public AppliedMethod AppliedMethod { get; private set; } = default!;

    public ICollection<DataApproval> Approvals { get; } = [];
    public ResponseApproval? Approval { get; set; }

    // TODO Exactly one resource must not have a parent and each other resource must have one from
    // this list and the graph must be connected. In other words, the resources must form a tree.
    public abstract ICollection<GetHttpsResource> Resources { get; }

    public DataAccessRights DataAccessRights { get; private set; } = new DataAccessRights();

    public PublishingState PublishingState { get; private set; } = PublishingState.PENDING;

    public abstract Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    );

    /// <inheritdoc/>
    public bool IsRestrictedByApplication(string applicationId)
    {
        return DataAccessRights.AllowedApplications is not null
            && DataAccessRights.AllowedApplications.Contains(applicationId);
    }

    /// <inheritdoc/>
    public bool IsRestrictedByInstitutions(IEnumerable<Guid> institutions)
    {
        return DataAccessRights.AllowedInstitutions is not null
            && DataAccessRights.AllowedInstitutions.Any(a =>
                institutions.Any(b =>
                    a.Equals(b)
                )
            );
    }

    /// <inheritdoc/>
    public bool IsRestrictedByUser(Guid uuid, uint alreadyAccesedCount)
    {
        if (DataAccessRights.AllowedUserAndQuantity is null)
        {
            return false;
        }
        if (DataAccessRights.AllowedUserAndQuantity.TryGetValue(uuid, out var limit))
        {
            return limit is not null && alreadyAccesedCount >= limit;
        }
        return true;
    }
}