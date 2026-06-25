using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using HotChocolate.Resolvers;
using HotChocolate.Authorization;
using static Database.ApiRequests.VerifyDatabase;
using Database.Authorization;

namespace Database.GraphQl.Databases;

// TODO [ExtendObjectType(nameof(Mutation))]
public sealed class VerifyDatabaseMutation
{
    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<VerifyDatabasePayload> VerifyDatabaseAsync(
        VerifyDatabaseInput input,
        VerifyDatabase verifyDatabase,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var databasePayload = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => verifyDatabase.Do(
                input,
                cancellationToken
            ),
            resolverContext,
            verifyDatabase.GetGraphQlEndpoint
        );
        if (databasePayload is null)
        {
            return new VerifyDatabasePayload(
                 null,
                 [
                     new VerifyDatabaseError(
                        VerifyDatabaseErrorCode.UNKNOWN,
                        "Unknown error.",
                        []
                    )
                 ]
            );
        }
        return databasePayload;
    }
}
