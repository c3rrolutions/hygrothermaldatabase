using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;
using Database.Data;
using Database.Enumerations;
using System.Linq;

namespace Database.GraphQl.DataX;

public sealed class DataDataLoaders
: DataLoaders
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<(Guid id, DataKind kind), IData>> GetDataByIdAndKindAsync(
        IReadOnlyList<(Guid id, DataKind kind)> idsAndKinds,
        Database.GraphQl.CalorimetricDataX.ICalorimetricDataByIdDataLoader calorimetricDataById,
        Database.GraphQl.HygrothermalDataX.IHygrothermalDataByIdDataLoader hygrothermalDataById,
        Database.GraphQl.LifeCycleDataX.ILifeCycleDataByIdDataLoader lifeCycleDataById,
        Database.GraphQl.OpticalDataX.IOpticalDataByIdDataLoader opticalDataById,
        Database.GraphQl.PhotovoltaicDataX.IPhotovoltaicDataByIdDataLoader photovoltaicDataById,
        Database.GraphQl.GeometricDataX.IGeometricDataByIdDataLoader geometricDataById,
        CancellationToken cancellationToken
    )
    {
        var allData = await Task.WhenAll(
            idsAndKinds
            .Select<(Guid id, DataKind kind), Task<IData?>>(async _ =>
                _.kind switch
                {
                    DataKind.CALORIMETRIC_DATA => await calorimetricDataById.LoadRequiredAsync(_.id, cancellationToken),
                    DataKind.GEOMETRIC_DATA => await geometricDataById.LoadRequiredAsync(_.id, cancellationToken),
                    DataKind.HYGROTHERMAL_DATA => await hygrothermalDataById.LoadRequiredAsync(_.id, cancellationToken),
                    DataKind.LIFE_CYCLE_DATA => await lifeCycleDataById.LoadRequiredAsync(_.id, cancellationToken),
                    DataKind.OPTICAL_DATA => await opticalDataById.LoadRequiredAsync(_.id, cancellationToken),
                    DataKind.PHOTOVOLTAIC_DATA => await photovoltaicDataById.LoadRequiredAsync(_.id, cancellationToken),
                    _ => null
                }
            )
        );
        return idsAndKinds.Zip(allData, (idAndKind, data) =>
                new { id = idAndKind.id, kind = idAndKind.kind, data }
            )
            .Where(_ => _.data is not null)
            .ToDictionary(_ => (_.id, _.kind), _ => _.data!);
    }
}