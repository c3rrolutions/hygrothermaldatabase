using Database.ApiRequests;
using Database.Data;
using Database.GraphQl.Entities;
using Database.Extensions;
using HotChocolate.Types;

namespace Database.GraphQl.GetHttpsResources;

public sealed class GetHttpsResourceType
    : EntityType<GetHttpsResource, IGetHttpsResourceByIdDataLoader>
{
    protected override void Configure(
        IObjectTypeDescriptor<GetHttpsResource> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field("locator")
            .Cost(0)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetLocator(default!, default!));
        descriptor
            .Field(_ => _.FileName)
            .Ignore();
        descriptor
            .Field(_ => _.FilePath)
            .Ignore();
        descriptor
            .Field(_ => _.FileExtension)
            .Ignore();
        descriptor
            .Field(x => x.ParentId)
            .Ignore();
        descriptor
            .Field(x => x.Parent)
            .Cost(0)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetParent(default!, default!, default!));
        descriptor
            .Field(x => x.Children)
            .Cost(0)
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
            .Field(x => x.GeometricDataId)
            .Ignore();
        descriptor
            .Field(x => x.GeometricData)
            .Ignore();
        descriptor
            .Field(x => x.HygrothermalDataId)
            .Ignore();
        descriptor
            .Field(x => x.HygrothermalData)
            .Ignore();
        descriptor
            .Field(x => x.LifeCycleDataId)
            .Ignore();
        descriptor
            .Field(x => x.LifeCycleData)
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
            .Field(x => x.Data)
            .Cost(0)
            .ResolveWith<GetHttpsResourceResolvers>(t =>
                t.GetData(default!, default!, default!, default!, default!, default!, default!, default!));
        descriptor
            .Field(nameof(GetHttpsResource.DataFormatId)[..^2].FirstCharToLower())
            .Type<ObjectType<DataFormatDataLoader.DataFormat>>()
            .Cost(3)
            .ResolveWith<GetHttpsResourceResolvers>(_ => _.GetDataFormatAsync(default!, default!));
    }
}