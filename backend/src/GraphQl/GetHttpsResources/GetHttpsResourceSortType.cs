using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.GetHttpsResources;

public class GetHttpsResourceSortType
    : EntitySortType<GetHttpsResource>
{
    protected override void Configure(
        ISortInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.HashValue);
        descriptor.Field(x => x.DataFormatId);
        descriptor.Field(x => x.AppliedConversionMethod);
        descriptor.Field(x => x.Parent);
    }
}