using System;
using Database.Data.AccessPolicies;

namespace Database.GraphQl.AccessPolicies;

public sealed record UpperLimitPerDurationInput(
    uint UpperLimit,
    TimeSpan? Duration
)
{
    public UpperLimitPerDuration ToDomainModel()
        => new(UpperLimit, Duration);
};