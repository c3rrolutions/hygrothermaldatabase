using System;
using System.Collections.Generic;
using System.Linq;
using Database.Enumerations;

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

    public DataAccessMode DataAccess { get; set; } = DataAccessMode.UNRESTRICTED;
    public DataAccessRights DataAccessRights { get; } = new DataAccessRights();

    public bool IsRestrictedByApplication(string applicationId)
    {
        if (this.DataAccess == DataAccessMode.UNRESTRICTED)
            return false;
        return this.DataAccessRights.AllowedApplications.Contains(applicationId);
    }

    public bool IsRestrictedByInstitutions(List<Guid> institutions)
    {
        if (this.DataAccess == DataAccessMode.UNRESTRICTED)
            return false;
        return this.DataAccessRights.AllowedInstitutions.Any(a => institutions.Any(b => a.Equals(b)));
    }

    public bool IsRestrictedByUser(Guid uuid, int alreadyAccesedCount)
    {
        if (this.DataAccess == DataAccessMode.UNRESTRICTED)
            return false;
        int limit;
        if (this.DataAccessRights.AllowedUserAndQuantity.TryGetValue(uuid, out limit))
        {
            if (limit < 0 || limit > alreadyAccesedCount)
            {
                return false;
            }
        }
        return true;
    }
}