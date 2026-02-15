using HotChocolate.Configuration;
using HotChocolate.Data.Filters;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;

namespace Database.GraphQl.Filters;

public sealed class NotField
    // : FilterOperationField
    : InputField
    , INotField
{
    internal NotField(IDescriptorContext context, int index, string? scope)
        : base(CreateDefinition(context, scope), index)
    {
    }

    public new FilterInputType DeclaringType => (FilterInputType)base.DeclaringType;

    IFilterInputType INotField.DeclaringType => DeclaringType;

    protected override void OnCompleteField(
        ITypeCompletionContext context,
        ITypeSystemMember declaringMember,
        InputFieldDefinition definition)
    {
        definition.Type = TypeReference.Parse(
            $"[{context.Type.Name}!]",
            TypeContext.Input,
            context.Type.Scope);

        base.OnCompleteField(context, declaringMember, definition);
    }

    private static FilterOperationFieldDefinition CreateDefinition(
        IDescriptorContext context,
        string? scope)
    {
        return FilterOperationFieldDescriptor
            .New(context, AdditionalFilterOperations.Not, scope)
            .CreateDefinition();
    }
}