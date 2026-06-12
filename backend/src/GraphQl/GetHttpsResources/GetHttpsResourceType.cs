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
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetLocator(default!, default!, default!));
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
            .Field(_ => _.ParentId)
            .Ignore();
        descriptor
            .Field(_ => _.Parent)
            .Cost(0)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetParent(default!, default!, default!));
        descriptor
            .Field(_ => _.Children)
            .Cost(0)
            .ResolveWith<GetHttpsResourceResolvers>(t => t.GetChildren(default!, default!, default!));
        descriptor
            .Field(_ => _.DataId)
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.CalorimetricData)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricDataId)
            .Ignore();
        descriptor
            .Field(_ => _.GeometricData)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.HygrothermalData)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleDataId)
            .Ignore();
        descriptor
            .Field(_ => _.LifeCycleData)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalDataId)
            .Ignore();
        descriptor
            .Field(_ => _.OpticalData)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicDataId)
            .Ignore();
        descriptor
            .Field(_ => _.PhotovoltaicData)
            .Ignore();
        descriptor
            .Field(_ => _.Data)
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