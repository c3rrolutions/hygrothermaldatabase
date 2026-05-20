using System;
using Database.Data;
using Database.GraphQl.Entities;
using Database.GraphQl.Scalars;
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
            .Field(x => x.Locale)
            .Type<NonNullType<LocaleType>>();
        descriptor
            .Field(x => x.Resources)
            .ResolveWith<DataResolvers>(t => t.GetHttpsResources(default!, default!, default!, default!));
        descriptor
            .Field(DataType.ResourceTreeFieldName)
            .ResolveWith<DataResolvers>(t => t.GetHttpsResourceTree(default!));
        descriptor
            .Field(x => x.Approval)
            .Type<NonNullType<ObjectType<ResponseApproval>>>();
    }
}