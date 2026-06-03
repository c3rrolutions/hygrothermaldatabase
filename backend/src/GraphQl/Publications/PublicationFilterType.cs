using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.Publications;

public sealed class PublicationFilterType
    : FilterInputType<Publication>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Publication> descriptor
    )
    {
        descriptor.Field(_ => _.Exists).Ignore();
        descriptor.Name(nameof(PublicationFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
    }
}
