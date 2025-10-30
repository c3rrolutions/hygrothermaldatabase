using Database.Data;
using HotChocolate.Types;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceType
    : EntityType<GetHttpsResource, GetHttpsResourceByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field("locator")
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetLocator(default!, default!));
        descriptor
            .Field(_ => _.FilePath)
            .Ignore();
        descriptor
            .Field(x => x.ParentId)
            .Ignore();
        descriptor
            .Field(x => x.Parent)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetParent(default!, default!, default!));
        descriptor
            .Field(x => x.Children)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetChildren(default!, default!, default!));
        descriptor
            .Field(x => x.DataId)
            .Ignore();
        descriptor
            .Field(x => x.CalorimetricDataId)
            .Ignore();
        descriptor
            .Field(x => x.CalorimetricData)
            .Ignore();
        descriptor
            .Field(x => x.HygrothermalDataId)
            .Ignore();
        descriptor
            .Field(x => x.HygrothermalData)
            .Ignore();
        descriptor
            .Field(x => x.OpticalDataId)
            .Ignore();
        descriptor
            .Field(x => x.OpticalData)
            .Ignore();
        descriptor
            .Field(x => x.PhotovoltaicDataId)
            .Ignore();
        descriptor
            .Field(x => x.PhotovoltaicData)
            .Ignore();
        descriptor
            .Field(x => x.GeometricDataId)
            .Ignore();
        descriptor
            .Field(x => x.GeometricData)
            .Ignore();
        descriptor
            .Field(x => x.Data)
            .ResolveWith<GetHttpsResourceResolvers>(t =>
                t.GetData(default!, default!, default!, default!, default!, default!, default!));
    }
}