using System;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.Projectables;

namespace Database.Data.AccessPolicies;

public sealed class InstitutionAccessPolicy(
    Guid dataAccessPolicyId,
    Guid institutionId
)
: AccessPolicyBase
{
    [InverseProperty(nameof(DataAccessPolicy.InstitutionAccessPolicies))]
    public override DataAccessPolicy? DataAccessPolicy { get; set; }

    public Guid DataAccessPolicyId { get; private set; } = dataAccessPolicyId;

    public Guid InstitutionId { get; private set; } = institutionId;

    [Projectable]
    public bool IsAccessAllowed(Guid[]? institutionIds) =>
        institutionIds != null
        && institutionIds.Contains(InstitutionId)
        && IsWithinAccessLimitInTimeSpan;
}