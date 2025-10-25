using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.GetHttpsResources;

public class GetHttpsResourceSortType
    : SortInputType<GetHttpsResource>
{
    protected override void Configure(
        ISortInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.HashValue);
        descriptor.Field(x => x.DataFormatId);
        descriptor.Field(x => x.AppliedConversionMethod);
        descriptor.Field(x => x.Parent);
    }
}