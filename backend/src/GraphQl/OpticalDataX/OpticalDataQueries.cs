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

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class OpticalDataQueries
: DataQueriesBase<OpticalData>
{
    [UsePaging]
    [UseFiltering<OpticalDataFilterType>]
    [UseSorting<OpticalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<OpticalData>> GetAllOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            databaseContext => databaseContext.OpticalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<OpticalDataFilterType>]
    [UseSorting<OpticalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<OpticalData>> GetAllPendingOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            databaseContext => databaseContext.OpticalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<OpticalDataFilterType>]
    public Task<bool> HasOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            databaseContext => databaseContext.OpticalData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<OpticalData?> GetOpticalDataAsync(
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
            databaseContext => databaseContext.OpticalData,
            databaseContextFactory,
            accessPolicyService,
            cancellationToken
        );
    }
}