using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Data.Sorting;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Database.Data;
using Database.GraphQl.CalorimetricDataX;
using Database.GraphQl.HygrothermalDataX;
using Database.GraphQl.OpticalDataX;
using Database.GraphQl.PhotovoltaicDataX;
using Database.GraphQl.GeometricDataX;
using Database.GraphQl.Extensions;

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
        ApplicationDbContext context,
        ISortingContext sorting
    )
    {
        sorting.StabilizeOrder<IData>();
        // TODO Use `locale`. return context.CalorimetricData.AsNoTracking<Data.IData>(); The union
        // below does sadly not work because the different kinds of data have different include
        // operations. return context.CalorimetricData.AsNoTracking<Data.IData>()
        // .Union(context.HygrothermalData.AsNoTracking<Data.IData>())
        // .Union(context.OpticalData.AsNoTracking<Data.IData>()) .Union(context.PhotovoltaicData.AsNoTracking<Data.IData>());
        return context.CalorimetricData.AsNoTracking<IData>().AsAsyncEnumerable()
            .Concat(context.GeometricData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.HygrothermalData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.OpticalData.AsNoTracking<IData>().AsAsyncEnumerable())
            .Concat(context.PhotovoltaicData.AsNoTracking<IData>().AsAsyncEnumerable());
    }

    public async Task<IData?> GetDataAsync(
        Guid id,
        [GraphQLType<LocaleType>] string? locale,
        CalorimetricDataByIdDataLoader calorimetricDataById,
        HygrothermalDataByIdDataLoader hygrothermalDataById,
        OpticalDataByIdDataLoader opticalDataById,
        PhotovoltaicDataByIdDataLoader photovoltaicDataById,
        GeometricDataByIdDataLoader geometricDataById,
        CancellationToken cancellationToken
    )
    {
        // TODO Use `locale`.
        return
            await calorimetricDataById.LoadAsync(
                id,
                cancellationToken
            ).ConfigureAwait(false) ??
            await hygrothermalDataById.LoadAsync(
                id,
                cancellationToken
            ).ConfigureAwait(false) ??
            await opticalDataById.LoadAsync(
                id,
                cancellationToken
            ).ConfigureAwait(false) ??
            await geometricDataById.LoadAsync(
                id,
                cancellationToken
            ).ConfigureAwait(false) ??
            await photovoltaicDataById.LoadAsync(
                id,
                cancellationToken
            ).ConfigureAwait(false) as IData;
    }
}