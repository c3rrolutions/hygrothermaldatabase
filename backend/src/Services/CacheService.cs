using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using static Database.ApiRequests.QueryCurrentUserOrInstitution;

namespace Database.Services;

public sealed class CacheService(
    IMemoryCache currentUserOrInstitutionCache
)
{
    public CurrentUserOrInstitution? SetCurrentUserOrInstitution(string token, CurrentUserOrInstitution userOrInstitution)
    {
        var cacheEntryOptions =
            new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromHours(1));
        return currentUserOrInstitutionCache.Set(token, userOrInstitution, cacheEntryOptions);
    }

    public bool TryGetCurrentUserOrInstitution(string token, [NotNullWhen(true)] out CurrentUserOrInstitution? cachedUserOrInstitution)
    {
        return currentUserOrInstitutionCache.TryGetValue(token, out cachedUserOrInstitution);
    }
}