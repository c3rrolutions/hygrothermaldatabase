using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Extensions;
using Database.Services;
using EntityFrameworkCore.Projectables;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Database.Data;

[Index(nameof(InstitutionId), IsUnique = true)]
public sealed class InstitutionAccessRights(
    Guid institutionId,
    uint? allowedUserCount,
    uint? allowedDatasetsPerTime,
    Duration period
)
: AuditableEntity
{
    public Guid InstitutionId { get; set; } = institutionId;
    public uint? AllowedUserCount { get; set; } = allowedUserCount;
    public uint? AllowedDatasetsPerTime { get; set; } = allowedDatasetsPerTime;
    public Duration Period { get; set; } = period;
    public List<Guid> UserAlreadyAccessed { get; private set; } = [];

    [NotMapped]
    [Projectable]
    public bool HasRestrictions =>
        AllowedDatasetsPerTime != null
        || AllowedUserCount != null;

    [NotMapped]
    [Projectable]
    public bool HasRestrictionsByTime => AllowedDatasetsPerTime != null;

    [NotMapped]
    [Projectable]
    public bool HasRestrictionsByUser => AllowedUserCount != null;

    internal bool IsDataRestrictedByTime(IData dataItem, IClock clock, CacheService cacheService, out string? reason)
    {
        var isRestricted = false;
        reason = null;
        // Check restriction for time period
        if (AllowedDatasetsPerTime is not null)
        {
            var accessesPerPeriod = cacheService.GetOrCreateAccessCountForPeriod(InstitutionId);
            if (accessesPerPeriod.StartTime + Period < clock.GetUtcNow())
            {
                if (accessesPerPeriod.Count >= AllowedDatasetsPerTime)
                {
                    isRestricted = true;
                    reason = $"Id: {dataItem.Id} Reason: Maximum amount of allowed datasets for institution {InstitutionId} reached.";
                }
                cacheService.AddAccessCountToPeriod(InstitutionId);
            }
            else
            {
                cacheService.SetNewTimePeriod(InstitutionId);
            }
        }
        return isRestricted;
    }

    internal bool IsDataRestrictedByUser(IData dataItem, Guid currentUserId, out string? reason)
    {
        var isRestricted = false;
        reason = null;
        // Check restriction for users per institution
        if (AllowedUserCount is not null)
        {
            if (UserAlreadyAccessed.Count >= AllowedUserCount)
            {
                isRestricted = true;
                reason = $"Id: {dataItem.Id} Reason: Maximum allowed users of institution {InstitutionId} reached.";
            }
            UserAlreadyAccessed.Add(currentUserId);
        }
        return isRestricted;
    }
}