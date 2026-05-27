using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Data;
using Database.Extensions;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl;

public sealed class CrossDatabaseDataReferenceType
    : ObjectType<CrossDatabaseDataReference>
{
    protected override void Configure(
        IObjectTypeDescriptor<CrossDatabaseDataReference> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(nameof(CrossDatabaseDataReference.DatabaseId)[..^2].FirstCharToLower())
            .Type<ObjectType<DatabaseDataLoader.Database>>()
            .Cost(3)
            .ResolveWith<Resolvers>(_ => Resolvers.GetDatabaseAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public static Task<DatabaseDataLoader.Database?> GetDatabaseAsync(
            [Parent] CrossDatabaseDataReference parent,
            IDatabaseByIdDataLoader byId
        )
        {
            return byId.LoadAsync(parent.DatabaseId);
        }
    }
}