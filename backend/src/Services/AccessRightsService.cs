using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Database.ApiRequests.QueryCurrentUserOrApplication;

namespace Database.Services;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Cannot apply access rights because the user or application is unknown. The fetched user ID is {UserId} and the application ID is {ApplicationId}.")]
    public static partial void UnknownUserOrApplication(this ILogger<AccessRightsService> logger, Guid? userId, string? applicationId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Restricted Item Id: {Id} Reason:  {Reason}")]
    public static partial void DataRestriction(this ILogger<AccessRightsService> logger, Guid Id, string reason);
}

public sealed class AccessRightsService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
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
        ApplicationDbContext? context = null;
        uint? alreadyAccessedByUserCount = null;
        IReadOnlyList<Guid>? institutionIds = null;
        IReadOnlyList<InstitutionAccessRights>? institutionAccessRights = null;

        var currentUserOrApplication = await userService.FetchCurrentUserOrApplicationAsync(cancellationToken);
        var openIdConnectClientId = userService.GetOpenIdConnectClientId();

        if (currentUserOrApplication.CurrentUser is not null)
        {
            context = dbContextFactory.CreateDbContext();
            alreadyAccessedByUserCount = cacheService.GetAccessCountForUser(currentUserOrApplication.CurrentUser.Uuid);
            institutionIds = currentUserOrApplication.CurrentUser.RepresentedInstitutions.Edges.Select(e => e.Node.Uuid).ToList().AsReadOnly();
            institutionAccessRights = await GetInstitutionAccessRightsAsync(institutionIds, context, cancellationToken);
        }
        else if (currentUserOrApplication.CurrentApplication is not null)
        {
            context = dbContextFactory.CreateDbContext();
            institutionIds = [currentUserOrApplication.CurrentApplication.Owner.Node.Uuid];
            institutionAccessRights = await GetInstitutionAccessRightsAsync(institutionIds, context, cancellationToken);
        }

        var result = ProcessData(data, currentUserOrApplication.CurrentUser, openIdConnectClientId, institutionIds, institutionAccessRights, alreadyAccessedByUserCount, cacheService);

        if (context is not null)
        {
            // Save InstitutionAccessRight changes
            await context.SaveChangesAsync(cancellationToken);
        }
        return result.AsQueryable<T>();
    }

    private static async Task<IReadOnlyList<InstitutionAccessRights>> GetInstitutionAccessRightsAsync(
        IReadOnlyList<Guid> institutionIds,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        return (
            await context.InstitutionAccessRights.AsQueryable()
                .Where(x => institutionIds.Contains(x.InstitutionId))
                .ToListAsync(cancellationToken)
        )
        .AsReadOnly();
    }

    private IEnumerable<T> ProcessData<T>(
        IQueryable<T> data,
        CurrentUser? currentUser,
        string? openIdConnectClientId,
        IReadOnlyList<Guid>? institutionIds,
        IReadOnlyList<InstitutionAccessRights>? institutionAccessRights,
        uint? alreadyAccessedByUserCount,
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
                        institutionIds is null
                        || dataItem.IsRestrictedByInstitutions(institutionIds)
                    )
                    )
                {
                    logger.DataRestriction(dataItem.Id, "Institution not allowed.");
                    continue;
                }
                if (dataItem.DataAccessRights.HasRestrictionsByUser
                    && (
                        currentUser is null
                        || alreadyAccessedByUserCount is null
                        || dataItem.IsRestrictedByUser(currentUser.Uuid, alreadyAccessedByUserCount ?? 0)
                    )
                )
                {
                    logger.DataRestriction(dataItem.Id, "Allowed accesses for user reached.");
                    continue;
                }
                if (currentUser is not null && alreadyAccessedByUserCount is not null)
                {
                    alreadyAccessedByUserCount = alreadyAccessedByUserCount + 1;
                    cacheService.SetAccessCountForUser(currentUser.Uuid, alreadyAccessedByUserCount ?? 0);
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