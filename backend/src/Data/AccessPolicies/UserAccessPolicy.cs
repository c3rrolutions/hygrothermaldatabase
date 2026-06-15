using System;
using System.ComponentModel.DataAnnotations.Schema;
using Database.ApiRequests;
using EntityFrameworkCore.Projectables;

namespace Database.Data.AccessPolicies;

public sealed class UserAccessPolicy(
    Guid dataAccessPolicyId,
    Guid userId
)
: AccessPolicyBase
{
    [InverseProperty(nameof(DataAccessPolicy.UserAccessPolicies))]
    public override DataAccessPolicy? DataAccessPolicy { get; set; }

    public Guid DataAccessPolicyId { get; private set; } = dataAccessPolicyId;

    public Guid UserId { get; private set; } = userId;

    [Projectable]
    public bool IsAccessAllowed(QueryCurrentUserOrInstitution.CurrentUser? currentUser) =>
        currentUser != null
        && UserId == currentUser.Uuid
        && IsWithinAccessLimitInTimeSpan;
}