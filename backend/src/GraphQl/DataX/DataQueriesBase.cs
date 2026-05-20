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
    protected async Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        AccessRightsService accessRightsService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var sortedAndFilteredData =
            data.AsNoTracking()
            .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
            .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder);
        if (await sortedAndFilteredData.AnyAsync(_ => _.DataAccessRights.HasRestrictions, cancellationToken))
        {
            sortedAndFilteredData = (await accessRightsService.ApplyAccessRightsOnData(sortedAndFilteredData, cancellationToken)).AsQueryable();
        }
        return await sortedAndFilteredData
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    protected async Task<HotChocolate.Types.Pagination.Connection<TData>> GetAllPendingDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        AccessRightsService accessRightsService,
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
        var sortedAndFilteredData =
            data.AsNoTracking()
            .Where(_ => _.PublishingState == Enumerations.PublishingState.PENDING)
            .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder);
        if (await sortedAndFilteredData.AnyAsync(_ => _.DataAccessRights.HasRestrictions, cancellationToken))
        {
            sortedAndFilteredData = (await accessRightsService.ApplyAccessRightsOnData(sortedAndFilteredData, cancellationToken)).AsQueryable();
        }
        return await sortedAndFilteredData
            .ToPageAsync(resolverContext.GetPagingArguments(), cancellationToken)
            .ToConnectionAsync();
    }

    protected Task<bool> HasDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        return data.AsNoTracking()
            .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
            .With(resolverContext.GetQueryContext<TData>(), Sorting.DefaultEntityOrder)
            .AnyAsync(cancellationToken);
    }

    protected async Task<TData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        GreenDonut.DataLoaderBase<Guid, TData> byId,
        AccessRightsService accessRightsService,
        CancellationToken cancellationToken
    )
    {
        var data = await byId.LoadAsync(
            id,
            cancellationToken
        );
        if (data is null)
        {
            return null;
        }
        if (!data.DataAccessRights.HasRestrictions)
        {
            return data;
        }
        return await accessRightsService.ApplyAccessRightsOnData(data, cancellationToken);
    }
}