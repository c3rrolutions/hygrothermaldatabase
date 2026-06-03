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

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Query))]
public sealed class GeometricDataQueries
: DataQueriesBase<GeometricData>
{
    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<GeometricData>> GetAllGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return GetAllDataAsync(
            context.GeometricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    [UsePaging]
    [UseFiltering<GeometricDataFilterType>]
    [UseSorting<GeometricDataSortType>]
    public Task<HotChocolate.Types.Pagination.Connection<GeometricData>> GetAllPendingGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        return GetAllPendingDataAsync(
            context.GeometricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            authorization,
            cancellationToken
        );
    }

    [UseFiltering<GeometricDataFilterType>]
    public Task<bool> HasGeometricDataAsync(
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext context,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return HasDataAsync(
            context.GeometricData,
            locale,
            context,
            accessPolicyService,
            resolverContext,
            cancellationToken
        );
    }

    public Task<GeometricData?> GetGeometricDataAsync(
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
            context.GeometricData,
            context,
            accessPolicyService,
            cancellationToken
        );
    }
}
