using System;

namespace Database.Data.AccessPolicies;

public sealed record UpperLimitPerDuration(
    uint UpperLimit,
    TimeSpan? Duration
);