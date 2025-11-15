using Database.Data;
using HotChocolate.Data.Sorting;

namespace Database.GraphQl.Entities;

public abstract class EntitySortType<TEntity>
    : SortInputType<TEntity>
    where TEntity : IEntity
{
    protected override void Configure(
        ISortInputTypeDescriptor<TEntity> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        // descriptor.Field(x => x.Version);
    }
}