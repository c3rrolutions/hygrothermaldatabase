using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests.Dto;
using Database.Data;
using Database.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Services;

public sealed class AccessRightsService(
    ApplicationDbContext context,
    UserService userService,
    CacheService cacheService,
    ILogger<AccessRightsService> logger)
{
    public async Task<T?> ApplyAccessRightsOnData<T>(T data, CancellationToken cancellationToken)
    where T : IData
    {
        return await (
            await ApplyAccessRightsOnData(new List<T> { data }.AsQueryable(), cancellationToken)
        ).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IQueryable<T>> ApplyAccessRightsOnData<T>(
        IQueryable<T> data,
        CancellationToken cancellationToken
    )
    where T : IData
    {
        IReadOnlyList<Guid>? representedInstitutionIds = null;
        IReadOnlyList<InstitutionAccessRights>? institutionAccessRights = null;
        uint? alreadyAccessedCount = null;

        var currentUser = await userService.GetCurrentUser(cancellationToken);
        var openIdConnectClientId = userService.GetOpenIdConnectClientId();

        if (currentUser is not null)
        {
            representedInstitutionIds = currentUser.RepresentedInstitutions.Edges.Select(e => e.Node.Uuid).ToList().AsReadOnly();
            institutionAccessRights = (await context.InstitutionAccessRights.AsQueryable()
                .Where(x => representedInstitutionIds.Contains(x.InstitutionId))
                .ToListAsync(cancellationToken))
                .AsReadOnly();
            alreadyAccessedCount = cacheService.GetAccessCountForUser(currentUser.Uuid);
        }

        var result = ProcessData(data, currentUser, openIdConnectClientId, representedInstitutionIds, institutionAccessRights, alreadyAccessedCount, cacheService);

        // Save InstitutionAccessRight changes
        await context.SaveChangesAsync(cancellationToken);
        return result.AsQueryable<T>();
    }

    private IEnumerable<T> ProcessData<T>(
        IQueryable<T> data,
        CurrentUserDto? currentUser,
        string? openIdConnectClientId,
        IReadOnlyList<Guid>? representedInstitutionIds,
        IReadOnlyList<InstitutionAccessRights>? institutionAccessRights,
        uint? alreadyAccessedCount,
        CacheService cacheService
    )
    where T : IData
    {
        foreach (var dataItem in data)
        {
            if (dataItem.DataAccessRights.HasRestrictions)
            {
                if (dataItem.DataAccessRights.HasRestrictionsByApplication
                    && (
                        openIdConnectClientId is null
                        || dataItem.IsRestrictedByApplication(openIdConnectClientId)
                    )
                )
                {
                    logger.DataRestriction(dataItem.Id, "Application not allowed.");
                    continue;
                }
                if (dataItem.DataAccessRights.HasRestrictionsByInstitution
                    && (
                        representedInstitutionIds is null
                        || dataItem.IsRestrictedByInstitutions(representedInstitutionIds)
                    )
                    )
                {
                    logger.DataRestriction(dataItem.Id, "Institution not allowed.");
                    continue;
                }
                if (dataItem.DataAccessRights.HasRestrictionsByUser
                    && (
                        currentUser is null
                        || alreadyAccessedCount is null
                        || dataItem.IsRestrictedByUser(currentUser.Uuid, alreadyAccessedCount ?? 0)
                    )
                )
                {
                    logger.DataRestriction(dataItem.Id, "Allowed accesses for user reached.");
                    continue;
                }
                if (currentUser is not null && alreadyAccessedCount is not null)
                {
                    alreadyAccessedCount = alreadyAccessedCount + 1;
                    cacheService.SetAccessCountForUser(currentUser.Uuid, alreadyAccessedCount ?? 0);
                }
                var reason = "";
                if (institutionAccessRights is not null
                    && institutionAccessRights.Any(x =>
                        (
                            x.HasRestrictionsByTime
                            && x.IsDataRestrictedByTime(dataItem, cacheService, out reason)
                        )
                        || (
                            x.HasRestrictionsByUser
                            && (
                                currentUser is null
                                || x.IsDataRestrictedByUser(dataItem, currentUser.Uuid, out reason)
                            )
                        )
                    )
                )
                {
                    logger.DataRestriction(dataItem.Id, reason);
                    continue;
                }
            }
            yield return dataItem;
        }
    }
}