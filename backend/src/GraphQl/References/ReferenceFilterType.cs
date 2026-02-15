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
        descriptor.Field(x => x.Standard);
        descriptor.Field(x => x.Publication);
    }
}