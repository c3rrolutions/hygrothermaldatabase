using System;
using Database.Extensions;
using EntityFrameworkCore.Projectables;
using NodaTime;

namespace Database.Data.AccessPolicies;

public abstract class AccessPolicyBase
    : AuditableEntity
{
    public abstract DataAccessPolicy? DataAccessPolicy { get; set; }

    public UpperLimitPerDuration? UpperAccessLimitPerTimeDuration { get; set; }
    public CountSinceTime? AccessCountSinceStartTime { get; set; }

    [Projectable]
    public bool IsAlwaysAllowed => UpperAccessLimitPerTimeDuration is null;

    [Projectable]
    public bool IsWithinAccessLimitInTimeSpan => UpperAccessLimitPerTimeDuration is null || (
        UpperAccessLimitPerTimeDuration.UpperLimit > 0 && (
            AccessCountSinceStartTime is null
            || !IsWithinTimeSpan
            || AccessCountSinceStartTime.AccessCount < UpperAccessLimitPerTimeDuration.UpperLimit
        )
    );

    [Projectable]
    public bool IsWithinTimeSpan =>
        AccessCountSinceStartTime is null
        || UpperAccessLimitPerTimeDuration is null
        || UpperAccessLimitPerTimeDuration.Duration is null
        || DateTimeOffset.UtcNow <= AccessCountSinceStartTime.StartTime + UpperAccessLimitPerTimeDuration.Duration;

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