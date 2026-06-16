using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataSortType
    : DataSortTypeBase<PhotovoltaicData>
{
    protected override void Configure(
        ISortInputTypeDescriptor<PhotovoltaicData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Id);
    }
}
