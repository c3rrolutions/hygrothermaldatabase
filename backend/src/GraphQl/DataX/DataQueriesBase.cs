using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Entities;
using Database.GraphQl.Extensions;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public abstract class DataQueriesBase<TData>
where TData : class, IData
{
    protected async Task<IEnumerable<TData>> GetAllDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        sorting.StabilizeOrder<TData>();
        var filteredData =
            data.AsNoTracking()
            .Where(_ => _.PublishingState != Enumerations.PublishingState.PENDING)
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
    }

    protected async Task<IEnumerable<TData>> GetAllPendingDataAsync(
        DbSet<TData> data,
        [GraphQLType<LocaleType>] string? locale,
        AccessRightsService accessRightsService,
        ISortingContext sorting,
        IResolverContext resolverContext,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return Enumerable.Empty<TData>().AsQueryable();
        }
        sorting.StabilizeOrder<TData>();
        var filteredData =
            data.AsNoTracking()
            .Where(_ => _.PublishingState == Enumerations.PublishingState.PENDING)
            .Sort(resolverContext)
            .Filter(resolverContext);
        if (!await filteredData.AnyAsync(x => x.DataAccessRights.HasRestrictions, cancellationToken))
        {
            return filteredData;
        }
        return await accessRightsService.ApplyAccessRightsOnData(filteredData, cancellationToken);
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
            .Filter(resolverContext)
            .AnyAsync(cancellationToken);
    }

    protected async Task<TData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        EntityByIdDataLoader<TData> byId,
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