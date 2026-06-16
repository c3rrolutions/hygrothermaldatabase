using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

public abstract class ResponseApprovalFilterType
: AuditableEntityFilterType<IData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(ResponseApprovalFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.UserId);
        descriptor.Field(_ => _.Locale);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.ComponentId);
        descriptor.Field(_ => _.CreatorId);
        descriptor.Field(_ => _.AppliedMethod);
        descriptor.Field(_ => _.Approvals);
        descriptor.Field(_ => _.Resources);
        descriptor.Field(_ => _.Warnings);
    }
}
