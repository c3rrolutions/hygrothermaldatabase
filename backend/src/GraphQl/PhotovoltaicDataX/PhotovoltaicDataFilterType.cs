using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataFilterType
    : DataFilterTypeBase<PhotovoltaicData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<PhotovoltaicData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(PhotovoltaicDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
    }
}