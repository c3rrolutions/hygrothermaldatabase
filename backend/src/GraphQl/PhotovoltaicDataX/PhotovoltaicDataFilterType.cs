using Database.Data;
using Database.GraphQl.DataX;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed class PhotovoltaicDataFilterType
    : DataFilterTypeBase<PhotovoltaicData>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<PhotovoltaicData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(PhotovoltaicDataFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);

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