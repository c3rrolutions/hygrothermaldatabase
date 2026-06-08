using System;
using System.ComponentModel.DataAnnotations.Schema;

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
}