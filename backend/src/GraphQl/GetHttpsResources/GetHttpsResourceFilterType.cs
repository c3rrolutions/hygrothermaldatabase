using HotChocolate.Data.Filters;
using Database.GraphQl.Entities;
using Database.Data;

namespace Database.GraphQl.GetHttpsResources;

public class GetHttpsResourceFilterType
    : EntityFilterType<GetHttpsResource>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.HashValue);
        descriptor.Field(x => x.DataFormatId);
        descriptor.Field(x => x.AppliedConversionMethod);
        descriptor.Field(x => x.ArchivedFilesMetaInformation);
        descriptor.Field(x => x.Parent);
        // descriptor.Field(x => x.Data);
        descriptor.Field(x => x.CalorimetricData);
        descriptor.Field(x => x.GeometricData);
        descriptor.Field(x => x.HygrothermalData);
        descriptor.Field(x => x.OpticalData);
        descriptor.Field(x => x.PhotovoltaicData);
    }
}