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

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Query))]
public sealed class HygrothermalDataQueries
: DataQueriesBase<HygrothermalData>
{
    [UsePaging]
    // [UseProjection] // We disabled projections because when requesting `id` all results had the
    // same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    [UseFiltering<HygrothermalDataFilterType>]
    [UseSorting<HygrothermalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<HygrothermalData>> GetAllHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.HygrothermalData,
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
    [UseFiltering<HygrothermalDataFilterType>]
    [UseSorting<HygrothermalDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<HygrothermalData>> GetAllPendingHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.HygrothermalData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<HygrothermalDataFilterType>]
    public Task<bool> HasHygrothermalDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.HygrothermalData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<HygrothermalData?> GetHygrothermalDataAsync(
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
            context.HygrothermalData,
            context,
            accessPolicyService,
            cancellationToken
        );
    }
}
