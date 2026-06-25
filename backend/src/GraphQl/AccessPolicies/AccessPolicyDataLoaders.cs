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
    public static ValueTask<IReadOnlyDictionary<Guid, DataAccessPolicy>> GetDataAccessPolicyByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<DataAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.DataAccessPolicies,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, UserAccessPolicy>> GetUserAccessPolicyByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<UserAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.UserAccessPolicies,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, InstitutionAccessPolicy>> GetInstitutionAccessPolicyByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<InstitutionAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.InstitutionAccessPolicies,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, OpenIdConnectApplicationAccessPolicy>> GetOpenIdConnectApplicationAccessPolicyByIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<OpenIdConnectApplicationAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetEntityByIdAsync(
            ids,
            databaseContext => databaseContext.OpenIdConnectApplicationAccessPolicies,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static async ValueTask<IReadOnlyDictionary<Guid, DataAccessPolicy>> GetDataAccessPolicyByDataIdAsync(
        IReadOnlyList<Guid> dataIds,
        QueryContext<DataAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        await using var databaseContext =
            databaseContextFactory.CreateDbContext();
        return await databaseContext.DataAccessPolicies
            .AsNoTrackingWithIdentityResolution()
            .Where(_ => dataIds.Contains(_.DataId ?? Guid.Empty))
            .With(queryContext, Sorting.DefaultEntityOrder)
            .ToDictionaryAsync(_ => _.DataId ?? Guid.Empty, cancellationToken);
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, UserAccessPolicy[]>> GetUserAccessPoliciesByDataAccessPolicyIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<UserAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.UserAccessPolicies,
            _ => _.DataAccessPolicyId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, InstitutionAccessPolicy[]>> GetInstitutionAccessPoliciesByDataAccessPolicyIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<InstitutionAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.InstitutionAccessPolicies,
            _ => _.DataAccessPolicyId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }

    [DataLoader]
    public static ValueTask<IReadOnlyDictionary<Guid, OpenIdConnectApplicationAccessPolicy[]>> GetOpenIdConnectApplicationAccessPoliciesByDataAccessPolicyIdAsync(
        IReadOnlyList<Guid> ids,
        QueryContext<OpenIdConnectApplicationAccessPolicy> queryContext,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        CancellationToken cancellationToken
    )
    {
        return GetManyByOneIdAsync(
            ids,
            (databaseContext) => databaseContext.OpenIdConnectApplicationAccessPolicies,
            _ => _.DataAccessPolicyId,
            queryContext,
            databaseContextFactory,
            cancellationToken
        );
    }
}