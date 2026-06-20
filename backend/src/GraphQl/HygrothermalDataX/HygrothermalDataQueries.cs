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

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class HygrothermalDataQueries
: DataQueriesBase<HygrothermalData>
{
    [UsePaging]
    [UseFiltering<HygrothermalDataFilterType>]
    [UseSorting<HygrothermalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<HygrothermalData>> GetAllHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            databaseContext => databaseContext.HygrothermalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<HygrothermalDataFilterType>]
    [UseSorting<HygrothermalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<HygrothermalData>> GetAllPendingHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            databaseContext => databaseContext.HygrothermalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<HygrothermalDataFilterType>]
    public Task<bool> HasHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            databaseContext => databaseContext.HygrothermalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<HygrothermalData?> GetHygrothermalDataAsync(
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
            databaseContext => databaseContext.HygrothermalData,
            databaseContextFactory,
            accessPolicyService,
            cancellationToken
        );
    }
}