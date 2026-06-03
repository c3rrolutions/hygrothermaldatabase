using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.GraphQl.Scalars;
using Database.Services;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public abstract class DataQueriesBase<TData>
where TData : class, IData
{
    protected Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext databaseContext,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.Apply(
            data.AsNoTracking()
                .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var connection = await policedData
                    .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                    .ToConnectionAsync();
                var nodes = connection.Edges.Select(_ => _.Node).ToList().AsReadOnly();
                return (nodes, connection);
            },
            databaseContext,
            cancellationToken
        );
    }

    protected async Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllPendingDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext databaseContext,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return await Enumerable.Empty<TData>()
                .AsQueryable()
                .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                .ToConnectionAsync();
        }
        return await accessPolicyService.Apply(
            data.AsNoTracking()
                .Where(_ => _.PublishingState == Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var connection = await policedData
                    .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
                    .ToConnectionAsync();
                var nodes = connection.Edges.Select(_ => _.Node).ToList().AsReadOnly();
                return (nodes, connection);
            },
            databaseContext,
            cancellationToken
        );
    }

    protected Task<bool> HasDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        ApplicationDbContext databaseContext,
        AccessPolicyService accessPolicyService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.Apply(
            data.AsNoTracking()
                .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
                .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder),
            async policedData =>
            {
                var nodes = (await policedData.ToListAsync(cancellationToken)).AsReadOnly();
                return (nodes, nodes.Count > 0);
            },
            databaseContext,
            cancellationToken
        );
    }

    protected Task<TData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        DbSet<TData> data,
        ApplicationDbContext databaseContext,
        AccessPolicyService accessPolicyService,
        CancellationToken cancellationToken
    )
    {
        return accessPolicyService.Apply<TData, TData?>(
            data.AsNoTracking()
                .Where(_ => _.Id == id),
            async policedData =>
            {
                var node = await policedData.SingleOrDefaultAsync(cancellationToken);
                return (node is null ? [] : [node], node);
            },
            databaseContext,
            cancellationToken
        );
    }
}