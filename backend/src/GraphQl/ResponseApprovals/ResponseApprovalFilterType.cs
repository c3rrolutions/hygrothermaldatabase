using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

public abstract class ResponseApprovalFilterType
: FilterInputType<ResponseApproval>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<ResponseApproval> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
    }
}