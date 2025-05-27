using System;
using System.Collections.Generic;
using System.Linq;

namespace Database.Data;

public abstract class DataX
    : Entity, IData
{
    protected DataX(
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

    // `DbContext` needs this constructor without owned entities.
    protected DataX(
        string locale,
        Guid componentId,
        string? name,
        string? description,
        string[] warnings,
        Guid creatorId,
        DateTime createdAt
    )
    {
        Locale = locale;
        ComponentId = componentId;
        Name = name;
        Description = description;
        Warnings = warnings;
        CreatorId = creatorId;
        CreatedAt = createdAt;
    }

    public string Locale { get; private set; }
    public Guid ComponentId { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string[] Warnings { get; private set; }
    public Guid CreatorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public AppliedMethod AppliedMethod { get; private set; } = default!;

    public ICollection<DataApproval> Approvals { get; } = new List<DataApproval>();
    public ResponseApproval? Approval { get; set; }

    // TODO Exactly one resource must not have a parent and each other resource must have one from
    // this list and the graph must be connected. In other words, the resources must form a tree.
    public virtual ICollection<GetHttpsResource> Resources { get; } = new List<GetHttpsResource>();

    public DataAccessRights DataAccessRights { get; } = new DataAccessRights();

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
        if (DataAccessRights.AllowedUserAndQuantity is not null)
        {
            if (DataAccessRights.AllowedUserAndQuantity.TryGetValue(uuid, out var limit))
            {
                if (limit is null || limit > alreadyAccesedCount)
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}