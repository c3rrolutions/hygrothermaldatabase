using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Database.ApiRequests;
using Database.Services;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Query))]
public sealed class DatabaseQueries
{
    public async Task<QueryDatabase.Database> GetDatabaseAsync(
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => QueryDatabase.Do(
                appSettings.DatabaseId,
                appSettings,
                apiRequestService,
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