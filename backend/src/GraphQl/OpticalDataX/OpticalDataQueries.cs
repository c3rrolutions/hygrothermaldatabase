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

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class OpticalDataQueries
: DataQueriesBase<OpticalData>
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<OpticalDataFilterType>]
    [UseSorting<OpticalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<OpticalData>> GetAllOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.OpticalData,
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
    [UseFiltering<OpticalDataFilterType>]
    [UseSorting<OpticalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<OpticalData>> GetAllPendingOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.OpticalData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<OpticalDataFilterType>]
    public Task<bool> HasOpticalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.OpticalData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<OpticalData?> GetOpticalDataAsync(
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
            context.OpticalData,
            context,
            accessPolicyService,
            cancellationToken
        );
    }
}
