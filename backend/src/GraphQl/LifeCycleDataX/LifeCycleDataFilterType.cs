using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataFilterType
    : DataFilterTypeBase<LifeCycleData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<LifeCycleData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(LifeCycleDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
    }
}