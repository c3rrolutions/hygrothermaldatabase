using HotChocolate.Types;
using GreenDonut;
using System;
using Database.Data;

namespace Database.GraphQl.DataX;

public abstract class DataTypeBase<TData, TDataByIdDataLoader>
    : EntityType<TData, TDataByIdDataLoader>
    where TData : IData
    where TDataByIdDataLoader : IDataLoader<Guid, TData?>
{
    protected override void Configure(
        IObjectTypeDescriptor<TData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(x => x.Locale)
            .Type<NonNullType<LocaleType>>();
        descriptor
            .Field(x => x.Resources)
            .ResolveWith<DataResolvers>(t => t.GetGetHttpsResources(default!, default!, default!, default!));
        descriptor
            .Field(DataType.ResourceTreeFieldName)
            .ResolveWith<DataResolvers>(t => t.GetGetHttpsResourceTree(default!));
        descriptor
            .Field(DataType.TimestampFieldName)
            .Type<NonNullType<DateTimeType>>()
            .ResolveWith<DataResolvers>(t => t.GetTimestamp());
        descriptor
            .Field(x => x.Approval)
            .Type<NonNullType<ObjectType<ResponseApproval>>>();
    }
}