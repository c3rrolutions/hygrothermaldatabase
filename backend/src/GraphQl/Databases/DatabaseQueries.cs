using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Query))]
public sealed class DatabaseQueries
{
    public async Task<QueryDatabase.Database> GetDatabaseAsync(
        AppSettings appSettings,
        QueryDatabase queryDatabase,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => queryDatabase.Do(
                appSettings.DatabaseId,
                cancellationToken
            ),
            resolverContext,
            queryDatabase.GetGraphQlEndpoint
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