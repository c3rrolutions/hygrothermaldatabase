using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.DataX;

public abstract class DataSortTypeBase<TData>
    : SortInputType<TData>
    where TData : IData
{
    protected override void Configure(
        ISortInputTypeDescriptor<TData> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Locale);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.ComponentId);
        descriptor.Field(x => x.CreatorId);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.AppliedMethod);
    }
}