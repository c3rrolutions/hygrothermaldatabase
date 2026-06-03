using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using GreenDonut.Data;
using Database.Data;
using Microsoft.EntityFrameworkCore;
using Database.Data.AccessPolicies;

namespace Database.GraphQl.AccessPolicies;

public sealed class InstitutionAccessPolicyLoaders
: DataLoaders
{
    [DataLoader]
    public static async ValueTask<IReadOnlyDictionary<Guid, UserAccessPolicy>> GetUserAccessPolicyByUserIdAsync(
        IReadOnlyList<Guid> userIds,
        QueryContext<UserAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        await using var databaseContext =
            databaseContextFactory.CreateDbContext();
        return await databaseContext.UserAccessPolicies
            .AsNoTrackingWithIdentityResolution()
            .Where(_ => userIds.Contains(_.UserId))
            .With(queryContext, _ => _.AddDescending(_ => _.UserId))
            .ToDictionaryAsync(_ => _.UserId, cancellationToken);
    }

    [DataLoader]
    public static async ValueTask<IReadOnlyDictionary<Guid, InstitutionAccessPolicy>> GetInstitutionAccessPolicyByInstitutionIdAsync(
        IReadOnlyList<Guid> institutionIds,
        QueryContext<InstitutionAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        await using var databaseContext =
            databaseContextFactory.CreateDbContext();
        return await databaseContext.InstitutionAccessPolicies
            .AsNoTrackingWithIdentityResolution()
            .Where(_ => institutionIds.Contains(_.InstitutionId))
            .With(queryContext, _ => _.AddDescending(_ => _.InstitutionId))
            .ToDictionaryAsync(_ => _.InstitutionId, cancellationToken);
    }

    [DataLoader]
    public static async ValueTask<IReadOnlyDictionary<string, OpenIdConnectApplicationAccessPolicy>> GetOpenIdConnectApplicationAccessPolicyByClientIdAsync(
        IReadOnlyList<string> clientIds,
        QueryContext<OpenIdConnectApplicationAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        await using var databaseContext =
            databaseContextFactory.CreateDbContext();
        return await databaseContext.OpenIdConnectApplicationAccessPolicies
            .AsNoTrackingWithIdentityResolution()
            .Where(_ => clientIds.Contains(_.ClientId ?? ""))
            .With(queryContext, _ => _.AddDescending(_ => _.ClientId))
            .ToDictionaryAsync(_ => _.ClientId ?? "", cancellationToken);
    }
}