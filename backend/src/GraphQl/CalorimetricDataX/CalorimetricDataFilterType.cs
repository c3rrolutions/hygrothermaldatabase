using Database.Data;
using Database.GraphQl.DataX;
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
        descriptor.Field(x => x.GValues);
        descriptor.Field(x => x.UValues);

        // TODO Why are the fields below not included by `base.Configure` above?
        // AuditableEntityFilterType.Configure
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.UpdatedAt);
        // DataFilterTypeBase.Configure
        descriptor.Field(x => x.UserId);
        descriptor.Field(x => x.Locale);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.ComponentId);
        descriptor.Field(x => x.CreatorId);
        descriptor.Field(x => x.AppliedMethod);
        descriptor.Field(x => x.Approvals);
        descriptor.Field(x => x.Resources);
        descriptor.Field(x => x.Warnings);
    }
}