using Database.Data;
using Database.GraphQl.DataX;
using Database.GraphQl.GetHttpsResources;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.HygrothermalDataX;

public sealed class HygrothermalDataFilterType
    : DataFilterTypeBase<HygrothermalData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<HygrothermalData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(HygrothermalDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);

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