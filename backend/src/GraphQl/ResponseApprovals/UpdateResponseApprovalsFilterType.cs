using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

public sealed class UpdateResponseApprovalsFilterType
: ResponseApprovalFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(UpdateResponseApprovalsFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}