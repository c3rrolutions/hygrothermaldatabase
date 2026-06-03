using System;

namespace Database.Data.AccessPolicies;

public sealed record CountSinceTime(
    uint AccessCount,
    DateTimeOffset? StartTime
);