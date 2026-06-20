using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.DataX;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.LifeCycleDataX;

[ExtendObjectType(nameof(Query))]
public sealed class LifeCycleDataQueries
: DataQueriesBase<LifeCycleData>
{
    [UsePaging]
    [UseFiltering<LifeCycleDataFilterType>]
    [UseSorting<LifeCycleDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<LifeCycleData>> GetAllLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            databaseContext => databaseContext.LifeCycleData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<LifeCycleDataFilterType>]
    [UseSorting<LifeCycleDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<LifeCycleData>> GetAllPendingLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            databaseContext => databaseContext.LifeCycleData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<LifeCycleDataFilterType>]
    public Task<bool> HasLifeCycleDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            databaseContext => databaseContext.LifeCycleData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<LifeCycleData?> GetLifeCycleDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        CancellationToken cancellationToken
    )
    {
        return GetDataAsync(
            id,
            locale,
            databaseContext => databaseContext.LifeCycleData,
            databaseContextFactory,
            accessPolicyService,
            cancellationToken
        );
    }
}