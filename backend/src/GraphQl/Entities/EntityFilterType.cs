using Database.Data;
using HotChocolate.Data.Filters;

namespace Database.GraphQl.Entities;

public abstract class EntityFilterType<TEntity>
    : FilterInputType<TEntity>
    where TEntity : IEntity
{
    protected override void Configure(
        IFilterInputTypeDescriptor<TEntity> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        // TODO Do we want to filter by: descriptor.Field(x => x.Version);
    }
}