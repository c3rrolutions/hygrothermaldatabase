using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Database.Data;
using Database.Enumerations;
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
        var filteredData = FilterData(data, openIdConnectClientId, currentUser, institutionIds, databaseContext);
        var (postProcessedData, result) = await then(filteredData);
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

    private static IQueryable<TData> FilterData<TData>(
        IQueryable<TData> data,
        string? openIdConnectClientId,
        QueryCurrentUserOrInstitution.CurrentUser? currentUser,
        IReadOnlyList<Guid>? institutionIds,
        ApplicationDbContext databaseContext
    )
        where TData : class, IData
    {
        return data.Where(_ =>
            _.AccessPolicy == null || !_.AccessPolicy.IsRestricted || (
                _.AccessPolicy.Combinator == LogicalCombinator.ALL && (
                    (!_.AccessPolicy.AreUsersRestricted || (
                        currentUser != null
                        && _.AccessPolicy.UserAccessPolicies != null
                        && _.AccessPolicy.UserAccessPolicies
                            .Where(_ => _.UserId == currentUser.Uuid)
                            .Any(_ => _.IsAllowed)
                        )
                    )
                    &&
                    (!_.AccessPolicy.AreInstitutionsRestricted || (
                        institutionIds != null
                        && _.AccessPolicy.InstitutionAccessPolicies != null
                        && _.AccessPolicy.InstitutionAccessPolicies
                            .Where(_ => institutionIds.Contains(_.InstitutionId))
                            .Any(_ => _.IsAllowed)
                        )
                    )
                    &&
                    (!_.AccessPolicy.AreOpenIdConnectApplicationsRestricted || (
                        openIdConnectClientId != null
                        && _.AccessPolicy.OpenIdConnectApplicationAccessPolicies != null
                        && _.AccessPolicy.OpenIdConnectApplicationAccessPolicies
                            .Where(_ => _.ClientId == openIdConnectClientId)
                            .Any(_ => _.IsAllowed)
                        )
                    )
                )
                ||
                _.AccessPolicy.Combinator == LogicalCombinator.SOME && (
                    (!_.AccessPolicy.AreUsersRestricted || (
                        currentUser != null
                        && _.AccessPolicy.UserAccessPolicies != null
                        && _.AccessPolicy.UserAccessPolicies
                            .Where(_ => _.UserId == currentUser.Uuid)
                            .Any(_ => _.IsAllowed)
                        )
                    )
                    ||
                    (!_.AccessPolicy.AreInstitutionsRestricted || (
                        institutionIds != null
                        && _.AccessPolicy.InstitutionAccessPolicies != null
                        && _.AccessPolicy.InstitutionAccessPolicies
                            .Where(_ => institutionIds.Contains(_.InstitutionId))
                            .Any(_ => _.IsAllowed)
                        )
                    )
                    ||
                    (!_.AccessPolicy.AreOpenIdConnectApplicationsRestricted || (
                        openIdConnectClientId != null
                        && _.AccessPolicy.OpenIdConnectApplicationAccessPolicies != null
                        && _.AccessPolicy.OpenIdConnectApplicationAccessPolicies
                            .Where(_ => _.ClientId == openIdConnectClientId)
                            .Any(_ => _.IsAllowed)
                        )
                    )
                )
            )
            &&
            (
                !databaseContext.UserAccessPolicies.AsQueryable().Any() || (
                    currentUser != null
                    && databaseContext.UserAccessPolicies.AsQueryable()
                        .Where(_ => _.UserId == currentUser.Uuid)
                        .Any(_ => _.IsAllowed)
                )
            )
            &&
            (
                !databaseContext.InstitutionAccessPolicies.AsQueryable().Any() || (
                    institutionIds != null
                    && databaseContext.InstitutionAccessPolicies.AsQueryable()
                        .Where(_ => institutionIds.Contains(_.InstitutionId))
                        .Any(_ => _.IsAllowed)
                )
            )
            &&
            (
                !databaseContext.OpenIdConnectApplicationAccessPolicies.AsQueryable().Any() || (
                    openIdConnectClientId != null
                    && databaseContext.OpenIdConnectApplicationAccessPolicies.AsQueryable()
                        .Where(_ => _.ClientId == openIdConnectClientId)
                        .Any(_ => _.IsAllowed)
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
        foreach (var data in allData)
        {
            databaseContext.Attach(data);
            if (currentUser is not null)
            {
                foreach (var userAccessPolicy in
                    data.AccessPolicy?.UserAccessPolicies
                        ?.Where(_ => _.UserId == currentUser.Uuid)
                        ?? []
                )
                {
                    userAccessPolicy?.IncrementAccessCount(clock);
                }
            }
            if (institutionIds is not null)
            {
                foreach (var institutionAccessPolicy in
                    data.AccessPolicy?.InstitutionAccessPolicies
                        ?.Where(_ => institutionIds.Contains(_.InstitutionId))
                        ?? []
                )
                {
                    institutionAccessPolicy?.IncrementAccessCount(clock);
                }
            }
            if (openIdConnectClientId is not null)
            {
                foreach (var applicationAccessPolicy in
                    data.AccessPolicy?.OpenIdConnectApplicationAccessPolicies
                        ?.Where(_ => _.ClientId == openIdConnectClientId)
                        ?? []
                )
                {
                    applicationAccessPolicy?.IncrementAccessCount(clock);
                }
            }
        }
        if (currentUser is not null)
        {
            foreach (var userAccessPolicy in
                await databaseContext.UserAccessPolicies.AsQueryable()
                    .Where(_ => _.UserId == currentUser.Uuid)
                    .ToListAsync(cancellationToken)
            )
            {
                userAccessPolicy?.IncrementAccessCount(clock);
            }
        }
        if (institutionIds is not null)
        {
            foreach (var institutionAccessPolicy in
                await databaseContext.InstitutionAccessPolicies.AsQueryable()
                    .Where(_ => institutionIds.Contains(_.InstitutionId))
                    .ToListAsync(cancellationToken)
            )
            {
                institutionAccessPolicy.IncrementAccessCount(clock);
            }
        }
        if (openIdConnectClientId is not null)
        {
            foreach (var applicationAccessPolicy in
                await databaseContext.OpenIdConnectApplicationAccessPolicies.AsQueryable()
                    .Where(_ => _.ClientId == openIdConnectClientId)
                    .ToListAsync(cancellationToken)
            )
            {
                applicationAccessPolicy?.IncrementAccessCount(clock);
            }
        }
        await databaseContext.SaveChangesAsync(cancellationToken);
    }
}