using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.References;

public sealed class ReferenceFilterType
    : FilterInputType<Reference>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Reference> descriptor
    )
    {

        descriptor.BindFieldsExplicitly();
        descriptor.Name(nameof(ReferenceFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.Standard);
        descriptor.Field(_ => _.Publication);
    }
}
