using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.GraphQl.Extensions;
using Database.Services;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

[ExtendObjectType(nameof(Query))]
public sealed class DataQueries
{
    [UsePaging] // TODO Paging does not work with this data source.
    // [UseProjection] // We disabled projections because when requesting `id` all results had the same `id` and when also requesting `uuid`, the latter was always the empty UUID `000...`.
    // [UseFiltering] // TODO Filtering does not work with unions.
    [UseSorting]
    public IAsyncEnumerable<IData> GetAllData(
        [GraphQLType<LocaleType>] string? locale,
        IDataService dataService,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<IData>();
        // TODO Use `locale`. return context.CalorimetricData.AsNoTracking<Data.IData>();
        return dataService.GetAllData();
    }

    public async Task<IData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        IDataService dataService,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        return await dataService.GetDataAsync(id, cancellationToken);
    }
}