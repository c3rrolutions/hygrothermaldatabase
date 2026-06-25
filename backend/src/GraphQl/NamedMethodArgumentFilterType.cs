using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl;

public sealed class NamedMethodArgumentFilterType
    : FilterInputType<NamedMethodArgument>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<NamedMethodArgument> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(NamedMethodArgumentFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.Name);
        // TODO descriptor.Field(_ => _.Value);
    }
}
