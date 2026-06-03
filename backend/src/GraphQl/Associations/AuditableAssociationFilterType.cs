using HotChocolate.Data.Filters;
using Database.Data;

namespace Database.GraphQl.Associations;

public abstract class AuditableAssociationFilterType<TAssociation>
    : FilterInputType<TAssociation>
    where TAssociation : IAssociation, IAuditable
{
    protected override void Configure(
        IFilterInputTypeDescriptor<TAssociation> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor.BindFieldsExplicitly();
        descriptor.Field(_ => _.CreatedAt);
        descriptor.Field(_ => _.UpdatedAt);
    }
}
