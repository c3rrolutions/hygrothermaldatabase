using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using static Database.ApiRequests.QueryCurrentUserOrApplication;

namespace Database.Services;

public sealed class CacheService(
    IMemoryCache currentUserOrApplicationCache
)
{
    public CurrentUserOrApplication? SetCurrentUserOrApplication(string token, CurrentUserOrApplication userOrApplication)
    {
        var cacheEntryOptions =
            new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
        return currentUserOrApplicationCache.Set(token, userOrApplication, cacheEntryOptions);
    }

    public bool TryGetCurrentUserOrApplication(string token, [NotNullWhen(true)] out CurrentUserOrApplication? cachedUserOrApplication)
    {
        return currentUserOrApplicationCache.TryGetValue(token, out cachedUserOrApplication);
    }
}