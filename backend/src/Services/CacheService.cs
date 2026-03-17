using System;
using System.Diagnostics.CodeAnalysis;
using Database.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NodaTime;
using static Database.ApiRequests.QueryCurrentUserOrInstitution;

namespace Database.Services;

public sealed class CacheService(
    IMemoryCache currentUserOrInstitutionCache,
    IMemoryCache accessCountCache,
    IMemoryCache timePeriodCountCache)
{
    public CurrentUserOrInstitution? SetCurrentUserOrInstitution(string token, CurrentUserOrInstitution cachedUserOrInstitution)
    {
        var cacheEntryOptions =
            new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
        return currentUserOrInstitutionCache.Set(token, cachedUserOrInstitution, cacheEntryOptions);
    }

    public bool TryGetCurrentUserOrInstitution(string token, [NotNullWhen(true)] out CurrentUserOrInstitution? cachedUserOrInstitution)
    {
        return currentUserOrInstitutionCache.TryGetValue(token, out cachedUserOrInstitution);
    }

    public uint GetAccessCountForUser(Guid userId)
    {
        if (!accessCountCache.TryGetValue(userId, out uint count))
        {
            return accessCountCache.Set<uint>(userId, 0);
        }
        return count;
    }

    public uint SetAccessCountForUser(Guid userId, uint count)
    {
        return accessCountCache.Set(userId, count);
    }

    public (OffsetDateTime StartTime, uint Count) GetOrCreateAccessCountForPeriod(Guid institutionId)
    {
        if (!timePeriodCountCache.TryGetValue(institutionId, out (OffsetDateTime StartTime, uint Count) accessesPerPeriod))
        {
            return timePeriodCountCache.Set(institutionId, (OffsetDateTime.UtcNow, (uint)0));
        }
        return accessesPerPeriod;
    }

    public (OffsetDateTime StartTime, uint Count) AddAccessCountToPeriod(Guid institutionId)
    {
        var accessesPerPeriod = GetOrCreateAccessCountForPeriod(institutionId);
        accessesPerPeriod.Count++;
        return timePeriodCountCache.Set(institutionId, accessesPerPeriod);
    }

    public (OffsetDateTime StartTime, uint Count) SetNewTimePeriod(Guid institutionId)
    {
        return timePeriodCountCache.Set(institutionId, (OffsetDateTime.UtcNow, (uint)0));
    }
}
