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

namespace Database.GraphQl.CalorimetricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class CalorimetricDataQueries
: DataQueriesBase<CalorimetricData>
{
    [UsePaging]
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<CalorimetricData>> GetAllCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            databaseContext => databaseContext.CalorimetricData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<CalorimetricData>> GetAllPendingCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            databaseContext => databaseContext.CalorimetricData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<CalorimetricDataFilterType>]
    public Task<bool> HasCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            databaseContext => databaseContext.CalorimetricData,
            locale,
            databaseContextFactory,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<CalorimetricData?> GetCalorimetricDataAsync(
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
            databaseContext => databaseContext.CalorimetricData,
            databaseContextFactory,
            accessPolicyService,
            cancellationToken
        );
    }
}