using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Database.Data;
using Database.ApiRequests;

namespace Database.Services;

public sealed class AccessPolicyService(
    UserService userService,
    IClock clock
)
{
    public async Task<TResult> Apply<TData, TResult>(
        IQueryable<TData> data,
        Func<IQueryable<TData>, Task<(IReadOnlyList<TData> Data, TResult Result)>> then,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
        where TData : class, IData
    {
        var openIdConnectClientId = userService.GetOpenIdConnectAuthorizedPartyClientId();
        var (currentUser, currentInstitution) = await userService.FetchCurrentUserOrInstitutionAsync(cancellationToken);
        IReadOnlyList<Guid>? institutionIds = null;
        if (currentUser is not null)
        {
            institutionIds = currentUser.RepresentedInstitutions.Edges.Select(e => e.Node.Uuid).ToList().AsReadOnly();
        }
        else if (currentInstitution is not null)
        {
            institutionIds = [currentInstitution.Uuid];
        }
        var policedData = PoliceData(data, currentUser, institutionIds, openIdConnectClientId, databaseContext);
        var (postProcessedData, result) = await then(policedData);
        await IncrementAccessCounts(
            postProcessedData,
            openIdConnectClientId,
            currentUser,
            institutionIds,
            databaseContext,
            cancellationToken
        );
        return result;
    }

    private static IQueryable<TData> PoliceData<TData>(
        IQueryable<TData> data,
        QueryCurrentUserOrInstitution.CurrentUser? currentUser,
        IReadOnlyList<Guid>? institutionIds,
        string? openIdConnectClientId,
        ApplicationDbContext databaseContext
    )
        where TData : class, IData
    {
        return data.Where(_ =>
            (
                _.AccessPolicy == null
                || _.AccessPolicy.IsAccessAllowed(currentUser, institutionIds, openIdConnectClientId)
            )
            &&
            (
                !databaseContext.DataAccessPolicies.Any(_ =>
                    _.DataId == null
                    && !_.IsAccessAllowed(currentUser, institutionIds, openIdConnectClientId)
                )
            )
        );
    }

    private async Task IncrementAccessCounts<TData>(
        IReadOnlyList<TData> allData,
        string? openIdConnectClientId,
        QueryCurrentUserOrInstitution.CurrentUser? currentUser,
        IReadOnlyList<Guid>? institutionIds,
        ApplicationDbContext databaseContext,
        CancellationToken cancellationToken
    )
        where TData : class, IData
    {
        if (currentUser is null && institutionIds is null && openIdConnectClientId is null)
        {
            return;
        }
        var allDataIds = allData.Select(_ => _.Id).ToArray();
        if (currentUser is not null)
        {
            foreach (var userAccessPolicy in
                await databaseContext.UserAccessPolicies
                    .Where(_ =>
                        _.DataAccessPolicy != null && (
                            _.DataAccessPolicy.DataId == null
                            || allDataIds.Contains(_.DataAccessPolicy.DataId ?? Guid.Empty)
                        )
                        && _.UserId == currentUser.Uuid
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