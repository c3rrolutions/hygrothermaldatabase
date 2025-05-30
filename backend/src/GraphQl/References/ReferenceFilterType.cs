using HotChocolate.Data.Filters;
using Database.Data;

namespace Database.GraphQl.References;

public sealed class ReferenceFilterType
    : FilterInputType<Reference>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Reference> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Standard);
        descriptor.Field(x => x.Publication);
    }
}