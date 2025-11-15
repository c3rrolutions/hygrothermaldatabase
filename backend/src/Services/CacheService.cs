using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using static Database.ApiRequests.QueryCurrentUserOrApplication;

namespace Database.Services;

public sealed class CacheService(
    IMemoryCache currentUserOrApplicationCache,
    IMemoryCache accessCountCache,
    IMemoryCache timePeriodCountCache)
{
    public CurrentUserOrApplication? SetCurrentUserOrApplication(string token, CurrentUserOrApplication cachedUserOrApplication)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
        return currentUserOrApplicationCache.Set(token, cachedUserOrApplication, cacheEntryOptions);
    }

    public bool TryGetCurrentUserOrApplication(string token, [NotNullWhen(true)] out CurrentUserOrApplication? cachedUserOrApplication)
    {
        return currentUserOrApplicationCache.TryGetValue(token, out cachedUserOrApplication);
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

    public (DateTime StartTime, uint Count) GetOrCreateAccessCountForPeriod(Guid institutionId)
    {
        if (!timePeriodCountCache.TryGetValue(institutionId, out (DateTime StartTime, uint Count) accessesPerPeriod))
        {
            return timePeriodCountCache.Set(institutionId, (DateTime.Now, (uint)0));
        }
        return accessesPerPeriod;
    }

    public (DateTime StartTime, uint Count) AddAccessCountToPeriod(Guid institutionId)
    {
        var accessesPerPeriod = GetOrCreateAccessCountForPeriod(institutionId);
        accessesPerPeriod.Count++;
        return timePeriodCountCache.Set(institutionId, accessesPerPeriod);
    }

    public (DateTime StartTime, uint Count) SetNewTimePeriod(Guid institutionId)
    {
        return timePeriodCountCache.Set(institutionId, (DateTime.Now, (uint)0));
    }
}