using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class ToTreeVertexAppliedConversionMethodType
    : ObjectType<ToTreeVertexAppliedConversionMethod>
{
    protected override void Configure(
        IObjectTypeDescriptor<ToTreeVertexAppliedConversionMethod> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(nameof(ToTreeVertexAppliedConversionMethod.MethodId)[..^2].FirstCharToLower())
            .Type<ObjectType<MethodDataLoader.Method>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetMethodAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<MethodDataLoader.Method?> GetMethodAsync(
            [Parent] ToTreeVertexAppliedConversionMethod parent,
            IMethodByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.MethodId);
        }
    }
}