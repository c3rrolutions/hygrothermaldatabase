using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.LifeCycleDataX;

public sealed class LifeCycleDataFilterType
    : DataFilterTypeBase<LifeCycleData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<LifeCycleData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(LifeCycleDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);

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
        descriptor.Field(_ => _.Resources);
        descriptor.Field(_ => _.Warnings);
    }
}
