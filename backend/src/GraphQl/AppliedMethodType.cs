using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class AppliedMethodType
    : ObjectType<AppliedMethod>
{
    protected override void Configure(
        IObjectTypeDescriptor<AppliedMethod> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(nameof(AppliedMethod.MethodId)[..^2].FirstCharToLower())
            .Type<ObjectType<MethodDataLoader.Method>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetMethodAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<MethodDataLoader.Method?> GetMethodAsync(
            [Parent] AppliedMethod parent,
            IMethodByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.MethodId);
        }
    }
}