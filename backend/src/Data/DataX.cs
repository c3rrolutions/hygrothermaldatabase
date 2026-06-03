using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Data.AccessPolicies;
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

    // The resources form a single, connected tree structure. Exactly one
    // resource acts as the root (having no parent), while every other resource
    // references a valid parent from this list. This structural integrity is
    // strictly enforced by PostgreSQL database constraints.
    public abstract ICollection<GetHttpsResource> Resources { get; }

    public DataAccessPolicy? AccessPolicy { get; set; }

    public PublishingState PublishingState { get; private set; } = PublishingState.PENDING;

    public abstract Task ExtractAndSetValuesFromFile(
        string filePath,
        Guid dataFormatId
    );
}