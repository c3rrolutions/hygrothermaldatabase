using Database.Data;
using HotChocolate.Types;
using Database.GraphQl.DataX;

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