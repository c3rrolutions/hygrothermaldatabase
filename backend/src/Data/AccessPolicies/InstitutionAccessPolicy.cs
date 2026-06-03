using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.AccessPolicies;

[PrimaryKey(nameof(InstitutionId))]
public sealed class InstitutionAccessPolicy(
    Guid institutionId
)
: AccessPolicyBase
{
    public Guid InstitutionId { get; private set; } = institutionId;
}