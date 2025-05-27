using System;
using System.Collections.Generic;
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
    /// Is data access ristricted by data access rights.
    /// </summary>
    public bool HasRistrictions
    {
        get
        {
            return AllowedUserAndQuantity is not null ||
                AllowedInstitutions is not null ||
                AllowedApplications is not null;
        }
    }

    /// <summary>
    /// Is data access ristricted by user.
    /// </summary>
    public bool IsRestrictedByUser
    {
        get
        {
            return AllowedUserAndQuantity is not null;
        }
    }

    /// <summary>
    /// Is data access ristricted by institution.
    /// </summary>
    public bool IsRestrictedByInstitution
    {
        get
        {
            return AllowedInstitutions is not null;
        }
    }

    /// <summary>
    /// Is data access ristricted by application.
    /// </summary>
    public bool IsRestrictedByApplication
    {
        get
        {
            return AllowedApplications is not null;
        }
    }
}