using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Types;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataType
    : DataTypeBase<PhotovoltaicData, PhotovoltaicDataByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<PhotovoltaicData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}