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
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.CalorimetricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<CalorimetricDataFilterType>]
    [UseSorting<CalorimetricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<CalorimetricData>> GetAllPendingCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.CalorimetricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<CalorimetricDataFilterType>]
    public Task<bool> HasCalorimetricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.CalorimetricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<CalorimetricData?> GetCalorimetricDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        CancellationToken cancellationToken
    )
    {
        return GetDataAsync(
            id,
            locale,
            context.CalorimetricData,
            context,
            accessPolicyService,
            cancellationToken
        );
    }
}
