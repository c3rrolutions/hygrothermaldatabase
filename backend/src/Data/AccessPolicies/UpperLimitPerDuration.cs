using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.AccessPolicies;

[Owned]
public sealed record UpperLimitPerDuration(
    uint UpperLimit,
    TimeSpan? Duration
);