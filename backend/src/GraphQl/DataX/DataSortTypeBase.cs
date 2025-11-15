using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.DataX;

public abstract class DataSortTypeBase<TData>
    : EntitySortType<TData>
    where TData : IData
{
    protected override void Configure(
        ISortInputTypeDescriptor<TData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(x => x.Locale);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.ComponentId);
        descriptor.Field(x => x.CreatorId);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.AppliedMethod);
    }
}