using System;
using Database.Extensions;
using EntityFrameworkCore.Projectables;
using NodaTime;

namespace Database.Data.AccessPolicies;

public abstract class AccessPolicyBase
{
    public UpperLimitPerDuration? UpperAccessLimitPerTimeDuration { get; set; }
    public CountSinceTime? AccessCountSinceStartTime { get; set; }

    [Projectable]
    public bool IsAllowed => UpperAccessLimitPerTimeDuration is null || (
        UpperAccessLimitPerTimeDuration.UpperLimit > 0 && (
            AccessCountSinceStartTime is null || (
                IsWithinTimeSpan
                && AccessCountSinceStartTime.AccessCount <= UpperAccessLimitPerTimeDuration.UpperLimit
            )
        )
    );

    [Projectable]
    public bool IsWithinTimeSpan =>
        AccessCountSinceStartTime is null
        || UpperAccessLimitPerTimeDuration is null
        || UpperAccessLimitPerTimeDuration.Duration is null
        || AccessCountSinceStartTime.StartTime + UpperAccessLimitPerTimeDuration.Duration < DateTimeOffset.UtcNow;

    public void IncrementAccessCount(IClock clock)
    {
        if (AccessCountSinceStartTime is null || !IsWithinTimeSpan)
        {
            AccessCountSinceStartTime = new(1, clock.GetUtcNow().ToDateTimeOffset());
        }
        else
        {
            AccessCountSinceStartTime = new(
                AccessCountSinceStartTime.AccessCount + 1,
                AccessCountSinceStartTime.StartTime
            );
        }
    }
}