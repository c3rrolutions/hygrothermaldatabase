using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using HotChocolate.Resolvers;
using HotChocolate.Authorization;
using HotChocolate.Types;
using static Database.ApiRequests.UpdateDatabase;
using Database.Authorization;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateDatabaseMutation
{
    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<UpdateDatabasePayload> UpdateDatabaseAsync(
        UpdateDatabaseInput input,
        UpdateDatabase updateDatabase,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var databasePayload = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => updateDatabase.Do(
                input,
                cancellationToken
            ),
            resolverContext,
            updateDatabase.GetGraphQlEndpoint
        );
        if (databasePayload is null)
        {
            return new UpdateDatabasePayload(
                 null,
                 [
                     new UpdateDatabaseError(
                        UpdateDatabaseErrorCode.UNKNOWN,
                        "Unknown error.",
                        []
                    )
                 ]
            );
        }
        return databasePayload;
    }
}
