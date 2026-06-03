using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.GetHttpsResources;

public class GetHttpsResourceSortType
    : AuditableEntitySortType<GetHttpsResource>
{
    protected override void Configure(
        ISortInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.HashValue);
        descriptor.Field(_ => _.DataFormatId);
        descriptor.Field(_ => _.AppliedConversionMethod);
        descriptor.Field(_ => _.Parent);
    }
}
