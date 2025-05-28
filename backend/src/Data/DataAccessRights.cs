using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

/// <summary>
/// Access rights per dataset
/// </summary>
[Owned]
public sealed class DataAccessRights
{
    /// <summary>
    /// Dictionary to store allowed user with the amount of a allowed dataset calls.
    /// If the user ID is mapped to `null`, then this means that the user has unlimited
    /// access.
    /// </summary>
    [Column(TypeName = "jsonb")]
    public IReadOnlyDictionary<Guid, uint?>? AllowedUserAndQuantity { get; set; }

    /// <summary>
    /// Allowed Institutions
    /// </summary>
    public IReadOnlyList<Guid>? AllowedInstitutions { get; set; }

    /// <summary>
    /// Allowed applications
    /// </summary>
    public IReadOnlyList<string>? AllowedApplications { get; set; }

    /// <summary>
    /// Is data access restricted by data access rights.
    /// </summary>
    [NotMapped]
    [Projectable]
    public bool HasRestrictions =>
        AllowedUserAndQuantity != null ||
        AllowedInstitutions != null ||
        AllowedApplications != null;

    /// <summary>
    /// Is data access restricted by user.
    /// </summary>
    [NotMapped]
    [Projectable]
    public bool IsRestrictedByUser => AllowedUserAndQuantity != null;

    /// <summary>
    /// Is data access restricted by institution.
    /// </summary>
    [NotMapped]
    [Projectable]
    public bool IsRestrictedByInstitution => AllowedInstitutions != null;

    /// <summary>
    /// Is data access restricted by application.
    /// </summary>
    [NotMapped]
    [Projectable]
    public bool IsRestrictedByApplication => AllowedApplications != null;
}