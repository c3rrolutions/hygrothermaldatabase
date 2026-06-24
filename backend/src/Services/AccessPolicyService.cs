using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Database.Data;
using Microsoft.Extensions.Logging;

namespace Database.Services;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Applying data access policy for user '{UserId}', institutions '{InstitutionIds}', and OpenID Connect application '{OpenIdConnectClientId}'")]
    internal static partial void ApplyingAccessPolicy(
        this ILogger<AccessPolicyService> logger,
        Guid? userId,
        Guid[]? institutionIds,
        string? openIdConnectClientId
    );
}

public sealed class AccessPolicyService(
    UserService userService,
    IClock clock,
    ILogger<AccessPolicyService> logger
)
{
    public async Task<TResult> Apply<TData, TResult>(
        Func<ApplicationDbContext, IQueryable<TData>> getData,
        Func<IQueryable<TData>, Task<(IReadOnlyList<TData> Data, TResult Result)>> then,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
        where TData : class, IData
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var openIdConnectClientId = userService.GetOpenIdConnectApplicationClientId();
        var (currentUser, currentInstitution) = await userService.FetchCurrentUserOrInstitutionAsync(cancellationToken);
        Guid[]? institutionIds = null;
        if (currentUser is not null)
        {
            institutionIds = currentUser.RepresentedInstitutions.Edges.Select(e => e.Node.Uuid).ToArray();
        }
        else if (currentInstitution is not null)
        {
            institutionIds = [currentInstitution.Uuid];
        }
        logger.ApplyingAccessPolicy(currentUser?.Uuid, institutionIds, openIdConnectClientId);
        var policedData = PoliceData(getData(databaseContext), currentUser?.Uuid, institutionIds, openIdConnectClientId, databaseContext);
        var (postProcessedData, result) = await then(policedData);
        await IncrementAccessCounts(
            postProcessedData,
            openIdConnectClientId,
            currentUser?.Uuid,
            institutionIds,
            databaseContext,
            cancellationToken
        );
        return result;
    }

    internal static IQueryable<TData> PoliceData<TData>(
        IQueryable<TData> data,
        Guid? userId,
        Guid[]? institutionIds,
        string? openIdConnectClientId,
        ApplicationDbContext databaseContext
    )
        where TData : class, IData
    {
        return data.Where(_ =>
            (
                _.AccessPolicy == null
                || _.AccessPolicy.IsAccessAllowed(userId, institutionIds, openIdConnectClientId)
            )
            &&
            (
                !databaseContext.DataAccessPolicies.Any(_ =>
                    _.DataId == null
                    && !_.IsAccessAllowed(userId, institutionIds, openIdConnectClientId)
                )
            )
        );
    }

    private async Task IncrementAccessCounts<TData>(
        IReadOnlyList<TData> allData,
        string? openIdConnectClientId,
        Guid? userId,
        Guid[]? institutionIds,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
        where TData : class, IData
    {
        if (userId is null && institutionIds is null && openIdConnectClientId is null)
        {
            return;
        }
        var allDataIds = allData.Select(_ => _.Id).ToArray();
        if (userId is not null)
        {
            foreach (var userAccessPolicy in
                await databaseContext.UserAccessPolicies
                    .Where(_ =>
                        _.DataAccessPolicy != null && (
                            _.DataAccessPolicy.DataId == null
                            || allDataIds.Contains(_.DataAccessPolicy.DataId ?? Guid.Empty)
                        )
                        && _.UserId == userId
                    )
                    .ToListAsync(cancellationToken)
            )
            {
                userAccessPolicy.IncrementAccessCount(clock);
            }
        }
        if (institutionIds is not null)
        {
            foreach (var institutionAccessPolicy in
                await databaseContext.InstitutionAccessPolicies
                    .Where(_ =>
                        _.DataAccessPolicy != null && (
                            _.DataAccessPolicy.DataId == null
                            || allDataIds.Contains(_.DataAccessPolicy.DataId ?? Guid.Empty)
                        )
                        && institutionIds.Contains(_.InstitutionId)
                    )
                    .ToListAsync(cancellationToken)
            )
            {
                institutionAccessPolicy.IncrementAccessCount(clock);
            }
        }
        if (openIdConnectClientId is not null)
        {
            foreach (var applicationAccessPolicy in
                await databaseContext.OpenIdConnectApplicationAccessPolicies
                    .Where(_ =>
                        _.DataAccessPolicy != null && (
                            _.DataAccessPolicy.DataId == null
                            || allDataIds.Contains(_.DataAccessPolicy.DataId ?? Guid.Empty)
                        )
                        && _.ClientId == openIdConnectClientId
                    )
                    .ToListAsync(cancellationToken)
            )
            {
                applicationAccessPolicy.IncrementAccessCount(clock);
            }
        }
        await databaseContext.SaveChangesAsync(cancellationToken);
    }
}