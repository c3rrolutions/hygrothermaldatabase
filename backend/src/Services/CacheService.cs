using System;
using Database.ApiRequests.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace Database.Services;

public class CacheService(
    IMemoryCache currentUserCache,
    IMemoryCache accessCountCache,
    IMemoryCache timePeriodCountCache) : ICacheService
{
    public CurrentUserDto? SetUser(string token, CurrentUserDto? cacheUser)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
        return currentUserCache.Set(token, cacheUser, cacheEntryOptions);
    }

    public bool TryGetUser(string token, out CurrentUserDto? cacheUser)
    {
        return currentUserCache.TryGetValue(token, out cacheUser);
    }

    public int GetAccessCountForUser(Guid userId)
    {
        int count;
        if (!accessCountCache.TryGetValue(userId, out count))
        {
            return accessCountCache.Set(userId, 0);
        }
        return count;
    }

    public int SetAccessCountForUser(Guid userId, int count)
    {
        return accessCountCache.Set(userId, count);
    }

    public (DateTime StartTime, int Count) GetOrCreateAccessCountForPerirod(Guid institutionId)
    {
        (DateTime StartTime, int Count) accessesPerPeriod;
        if (!timePeriodCountCache.TryGetValue(institutionId, out accessesPerPeriod))
        {
            return timePeriodCountCache.Set(institutionId, (DateTime.Now, 0));
        }
        return accessesPerPeriod;
    }

    public (DateTime StartTime, int Count) AddAccessCountToPeriod(Guid institutionId)
    {
        var accessesPerPeriod = GetOrCreateAccessCountForPerirod(institutionId);
        accessesPerPeriod.Count++;
        return timePeriodCountCache.Set(institutionId, accessesPerPeriod);
    }

    public (DateTime StartTime, int Count) SetNewTimePeriod(Guid institutionId)
    {
        return timePeriodCountCache.Set(institutionId, (DateTime.Now, 0));
    }
}