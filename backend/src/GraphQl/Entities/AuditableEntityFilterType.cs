using HotChocolate.Data.Filters;
using Database.Data;

namespace Database.GraphQl.Entities;

public abstract class AuditableEntityFilterType<TEntity>
    : FilterInputType<TEntity>
    where TEntity : IEntity, IAuditable
{
    protected override void Configure(
        IFilterInputTypeDescriptor<TEntity> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.CreatedAt);
        descriptor.Field(x => x.UpdatedAt);
        // TODO Do we want to filter by: descriptor.Field(x => x.Version);
    }

    // Inspired by https://github.com/ChilliCream/graphql-platform/blob/bc5a190019d8d0fbe46a557a59feacd7b30dd0c3/src/HotChocolate/Data/src/Data/Filters/FilterInputType.cs#L78
    // protected override FieldCollection<InputField> OnCompleteFields(
    //     ITypeCompletionContext context,
    //     InputObjectTypeDefinition definition
    // )
    // {
    //     var fields = new InputField[definition.Fields.Count + 2];
    //     var index = 0;
    //     if (definition is FilterInputTypeDefinition { UseAnd: true, } def)
    //     {
    //         fields[index] = new AndField(context.DescriptorContext, index, def.Scope);
    //         index++;
    //     }
    //     if (definition is FilterInputTypeDefinition { UseOr: true, } defOr)
    //     {
    //         fields[index] = new OrField(context.DescriptorContext, index, defOr.Scope);
    //         index++;
    //     }
    //     foreach (var fieldDefinition in
    //         definition.Fields.Where(t => !t.Ignore))
    //     {
    //         switch (fieldDefinition)
    //         {
    //             case FilterOperationFieldDefinition operation:
    //                 fields[index] = new FilterOperationField(operation, index);
    //                 index++;
    //                 break;
    //             case FilterFieldDefinition field:
    //                 fields[index] = new FilterField(field, index);
    //                 index++;
    //                 break;
    //         }
    //     }
    //     if (fields.Length > index)
    //     {
    //         Array.Resize(ref fields, index);
    //     }
    //     return CompleteFields(context, this, fields);
    // }
}