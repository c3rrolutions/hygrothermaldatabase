using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Enumerations;
using EntityFrameworkCore.Projectables;

namespace Database.Data.AccessPolicies;

public sealed class DataAccessPolicy
{
    public LogicalCombinator Combinator { get; set; } = LogicalCombinator.ALL;

    public List<UserAccessPolicy>? UserAccessPolicies { get; set; }
    public List<InstitutionAccessPolicy>? InstitutionAccessPolicies { get; set; }
    public List<OpenIdConnectApplicationAccessPolicy>? OpenIdConnectApplicationAccessPolicies { get; set; }

    [NotMapped]
    [Projectable]
    public bool IsRestricted =>
        UserAccessPolicies is not null ||
        InstitutionAccessPolicies is not null ||
        OpenIdConnectApplicationAccessPolicies is not null;

    [NotMapped]
    [Projectable]
    public bool AreUsersRestricted => UserAccessPolicies is not null;

    [NotMapped]
    [Projectable]
    public bool AreInstitutionsRestricted => InstitutionAccessPolicies is not null;

    [NotMapped]
    [Projectable]
    public bool AreOpenIdConnectApplicationsRestricted => OpenIdConnectApplicationAccessPolicies is not null;
}