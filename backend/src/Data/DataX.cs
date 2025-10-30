using System;
using System.Collections.Generic;
using System.Linq;
using Database.Enumerations;

namespace Database.Data;

public abstract class DataX(
    Guid userId,
    string locale,
    Guid componentId,
    string? name,
    string? description,
    string[] warnings,
    Guid creatorId,
    DateTime createdAt
) : Entity, IData
{
    protected DataX(
        Guid userId,
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt,
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
        DateTime createdAt,
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

    public Guid UserId { get; private set; } = userId;
    public string Locale { get; private set; } = locale;
    public Guid ComponentId { get; private set; } = componentId;
    public string? Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public string[] Warnings { get; private set; } = warnings;
    public Guid CreatorId { get; private set; } = creatorId;
    public DateTime CreatedAt { get; private set; } = createdAt;
    public AppliedMethod AppliedMethod { get; private set; } = default!;

    public ICollection<DataApproval> Approvals { get; } = [];
    public ResponseApproval? Approval { get; set; }

    // TODO Exactly one resource must not have a parent and each other resource must have one from
    // this list and the graph must be connected. In other words, the resources must form a tree.
    public virtual ICollection<GetHttpsResource> Resources { get; } = [];

    public DataAccessRights DataAccessRights { get; private set; } = new DataAccessRights();

    public PublishingState PublishingState { get; private set; } = PublishingState.PENDING;

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