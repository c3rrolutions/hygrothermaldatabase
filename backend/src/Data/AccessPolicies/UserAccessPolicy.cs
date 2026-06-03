using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.AccessPolicies;

[PrimaryKey(nameof(UserId))]
public sealed class UserAccessPolicy(
    Guid userId
)
: AccessPolicyBase
{
    public Guid UserId { get; private set; } = userId;
}