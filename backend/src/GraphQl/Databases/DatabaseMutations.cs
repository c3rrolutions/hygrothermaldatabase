using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Services;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using static Database.ApiRequests.UpdateDatabase;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Mutation))]
public sealed class DatabaseMutations
{
    public async Task<UpdateDatabasePayload> UpdateDatabaseAsync(
        UpdateDatabaseInput input,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var databasePayload = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => UpdateDatabase.Do(
                input,
                appSettings,
                apiRequestService,
                httpClientFactory,
                httpContextAccessor,
                cancellationToken
            ),
            resolverContext,
            UpdateDatabase.GetGraphQlEndpoint(appSettings)
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