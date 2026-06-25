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
    public async Task<DatabaseDataLoader.Database> GetDatabaseAsync(
        AppSettings appSettings,
        IDatabaseByIdDataLoader byId,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => byId.LoadAsync(
                appSettings.DatabaseId,
                cancellationToken
            ),
            resolverContext,
            DatabaseDataLoader.GetGraphQlEndpoint(appSettings)
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