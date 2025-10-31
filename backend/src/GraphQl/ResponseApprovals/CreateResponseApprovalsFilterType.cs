using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

public sealed class CreateResponseApprovalsFilterType
: ResponseApprovalFilterType
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(CreateResponseApprovalsFilterType)[..^10] + GraphQlConstants.FilterInputSuffix);
    }
}