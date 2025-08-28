using System;
using Database.ApiRequests.Dto;
using Microsoft.Extensions.Caching.Memory;

namespace Database.Services;

public sealed class CacheService(
    IMemoryCache currentUserCache,
    IMemoryCache accessCountCache,
    IMemoryCache timePeriodCountCache)
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