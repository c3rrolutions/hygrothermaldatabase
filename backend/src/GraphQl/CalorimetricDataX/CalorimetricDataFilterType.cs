using Database.Data;
using Database.GraphQl.DataX;
using Database.GraphQl.GetHttpsResources;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.CalorimetricDataX;

public sealed class CalorimetricDataFilterType
    : DataFilterTypeBase<CalorimetricData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<CalorimetricData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(CalorimetricDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.GValues);
        descriptor.Field(_ => _.UValues);

        // TODO Why are the fields below not included by `base.Configure` above?
        // AuditableEntityFilterType.Configure
        descriptor.Field(_ => _.Id);
        descriptor.Field(_ => _.CreatedAt);
        descriptor.Field(_ => _.UpdatedAt);
        // DataFilterTypeBase.Configure
        descriptor.Field(_ => _.UserId);
        descriptor.Field(_ => _.Locale);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.ComponentId);
        descriptor.Field(_ => _.CreatorId);
        descriptor.Field(_ => _.AppliedMethod);
        descriptor.Field(_ => _.Approvals);
        descriptor
            .Field(_ => _.Resources)
            .Type<ListFilterInputType<GetHttpsResourceFilterType>>();
        descriptor.Field(_ => _.Warnings);
    }
}