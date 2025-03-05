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
    /// Dictionary to store allowed user with the amount of a allowed dataset calls
    /// -1 meens unlimited access.
    /// </summary>
    public IDictionary<Guid, int> AllowedUserAndQuantity { get; set; }

    /// <summary>
    /// Allowed Institutions
    /// </summary>
    public ICollection<Guid> AllowedInstitutions { get; set; }

    /// <summary>
    /// Allowed applications
    /// </summary>
    public ICollection<string> AllowedApplications { get; set; }

    public DataAccessRights()
    {
        AllowedUserAndQuantity = new Dictionary<Guid, int>();
        AllowedInstitutions = new List<Guid>();
        AllowedApplications = new List<string>();
    }
}