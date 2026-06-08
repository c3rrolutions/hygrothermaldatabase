using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data.AccessPolicies;

[Owned]
public sealed record CountSinceTime(
    uint AccessCount,
    DateTimeOffset? StartTime
);