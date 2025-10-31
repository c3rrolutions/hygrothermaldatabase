using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.ResponseApprovals;

public abstract class ResponseApprovalFilterType
: FilterInputType<IData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<IData> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Locale);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.ComponentId);
        descriptor.Field(x => x.CreatorId);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.AppliedMethod);
        descriptor.Field(x => x.Approvals);
        descriptor.Field(x => x.Resources);
        descriptor.Field(x => x.Warnings);
    }
}