using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.GetHttpsResources;

public class GetHttpsResourceFilterType
    : AuditableEntityFilterType<GetHttpsResource>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Name(nameof(GetHttpsResourceFilterType)[..^"FilterType".Length] + GraphQlConstants.FilterInputSuffix);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.HashValue);
        descriptor.Field(_ => _.DataFormatId);
        descriptor.Field(_ => _.AppliedConversionMethod);
        descriptor.Field(_ => _.ArchivedFilesMetaInformation);
        descriptor.Field(_ => _.Parent);
        descriptor.Field(_ => _.DataId);
        descriptor.Field(_ => _.DataKind);
        // descriptor.Field(_ => _.Data);
        descriptor.Field(_ => _.CalorimetricData);
        descriptor.Field(_ => _.GeometricData);
        descriptor.Field(_ => _.HygrothermalData);
        descriptor.Field(_ => _.LifeCycleData);
        descriptor.Field(_ => _.OpticalData);
        descriptor.Field(_ => _.PhotovoltaicData);
    }
}