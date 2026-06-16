using System;
using Database.ApiRequests;
using Database.Data;
using Database.GraphQl.Entities;
using Database.GraphQl.Scalars;
using Database.Extensions;
using GreenDonut;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

public abstract class DataTypeBase<TData, TDataByIdDataLoader>
    : EntityType<TData, TDataByIdDataLoader>
    where TData : IData
    where TDataByIdDataLoader : IDataLoader<Guid, TData>
{
    protected override void Configure(
        IObjectTypeDescriptor<TData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(_ => _.PublishingState)
            .Ignore();
        descriptor
            .Field(_ => _.UpdatedAt)
            .Name(DataType.TimestampFieldName);
        descriptor
            .Field(_ => _.Locale)
            .Type<NonNullType<LocaleType>>();
        descriptor
            .Field(_ => _.Resources)
            .Cost(1)
            .ResolveWith<DataResolvers>(_ => _.GetHttpsResourcesAsync(default!, default!, default!, default!));
        descriptor
            .Field(DataType.ResourceTreeFieldName)
            .Cost(1)
            .ResolveWith<DataResolvers>(_ => _.GetHttpsResourceTree(default!));
        descriptor
            .Field(_ => _.AccessPolicy)
            .Cost(1)
            .ResolveWith<DataResolvers>(_ => _.GetDataAccessPolicyAsync(default!, default!, default!, default!, default!));
        descriptor
            .Field(nameof(DataResolvers.IsAnyoneAllowedAsync)[..^"Async".Length])
            .Cost(0)
            .ResolveWith<DataResolvers>(_ => DataResolvers.IsAnyoneAllowedAsync(default!, default!, default!));
        descriptor
            .Field(nameof(DataResolvers.IsAccessAllowedAsync)[..^"Async".Length])
            .Cost(0)
            .ResolveWith<DataResolvers>(_ => DataResolvers.IsAccessAllowedAsync(default!, default!, default!, default!, default!, default!));
        descriptor
            .Field(_ => _.Approval)
            .Cost(3)
            .Type<NonNullType<ObjectType<ResponseApproval>>>();
        descriptor
            .Field(DataType.DatabaseIdFieldName[..^2].FirstCharToLower())
            .Type<ObjectType<DatabaseDataLoader.Database>>()
            .Cost(3)
            .ResolveWith<DataResolvers>(_ => _.GetDatabaseAsync(default!, default!, default!));
        descriptor
            .Field(nameof(IData.ComponentId)[..^2].FirstCharToLower())
            .Type<ObjectType<ComponentDataLoader.Component>>()
            .Cost(3)
            .ResolveWith<DataResolvers>(_ => _.GetComponentAsync(default!, default!));
        descriptor
            .Field(nameof(IData.CreatorId)[..^2].FirstCharToLower())
            .Type<ObjectType<InstitutionDataLoader.Institution>>()
            .Cost(3)
            .ResolveWith<DataResolvers>(_ => _.GetInstitutionAsync(default!, default!));
    }
}