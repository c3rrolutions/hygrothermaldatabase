using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.DataX;

public sealed class DataFilterType
    : DataFilterTypeBase<IData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
    }
}