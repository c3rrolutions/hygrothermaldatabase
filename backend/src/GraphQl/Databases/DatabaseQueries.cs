using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Services;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Query))]
public sealed class DatabaseQueries
{
    public async Task<QueryDatabase.Database> GetDatabaseAsync(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => QueryDatabase.Do(
                appSettings.DatabaseId,
                appSettings,
                apiRequestService,
                httpClientFactory,
                httpContextAccessor,
                cancellationToken
            ),
            resolverContext,
            QueryDatabase.GetGraphQlEndpoint(appSettings)
        );
        if (database is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetPath(resolverContext.Path)
                    .SetMessage($"Failed to fetch database from the metabase GraphQl endpoint.")
                    .Build()
            );
        }
        return database;
    }
}