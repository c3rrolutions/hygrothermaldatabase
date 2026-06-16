using Database.Data;
using Database.GraphQl.Entities;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.DataX;

public abstract class DataSortTypeBase<TData>
    : AuditableEntitySortType<TData>
    where TData : IData, IAuditable
{
    protected override void Configure(
        ISortInputTypeDescriptor<TData> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.Field(_ => _.Locale);
        descriptor.Field(_ => _.Name);
        descriptor.Field(_ => _.Description);
        descriptor.Field(_ => _.ComponentId);
        descriptor.Field(_ => _.CreatorId);
        descriptor.Field(_ => _.AppliedMethod);
    }
}
