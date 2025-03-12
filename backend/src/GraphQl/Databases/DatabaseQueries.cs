using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Metabase;
using GraphQL;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Databases;

[ExtendObjectType(nameof(Query))]
public sealed class DatabaseQueries
{
    private static readonly string[] _databaseFileNames =
    {
        "Databases.graphql"
    };

    public async Task<Database> GetDatabaseAsync(
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var response =
                await QueryingMetabase.QueryGraphQl<DatabasesData>(
                    appSettings,
                    new GraphQLRequest(
                        await QueryingMetabase.ConstructGraphQlQuery(
                            _databaseFileNames
                        ).ConfigureAwait(false),
                        new
                        {
                            where = new
                            {
                                locator = new
                                {
                                    // TODO This is error-prone.
                                    absoluteUri = new
                                    {
                                        equalTo = new Uri(new Uri(appSettings.Host), "/graphql/")
                                    }
                                }
                            }
                        },
                        "Databases"
                    ),
                    httpClientFactory,
                    httpContextAccessor,
                    cancellationToken
                ).ConfigureAwait(false);
            if (response is null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetCode("NULL_RESPONSE")
                        .SetPath(resolverContext.Path)
                        .SetMessage("Response is null.")
                        .Build()
                );
            if (response.Data.Databases.Edges is null)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetCode("NULL_EDGES")
                        .SetPath(resolverContext.Path)
                        .SetMessage("The supposed list of databases is null.")
                        .Build()
                );
            if (response.Data.Databases.Edges.Count == 0)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetCode("NO_DATABASE")
                        .SetPath(resolverContext.Path)
                        .SetMessage("The list of databases is empty.")
                        .Build()
                );
            if (response.Data.Databases.Edges.Count >= 2)
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetCode("AMBIGUOUS_DATABASE")
                        .SetPath(resolverContext.Path)
                        .SetMessage("The list of databases has more than one entry.")
                        .Build()
                );
            return response.Data.Databases.Edges[0].Node;
        }
        catch (HttpRequestException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("METABASE_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage($"Failed with status code {e.StatusCode} to request the metabase GraphQl endpoint.")
                    .SetException(e)
                    .Build()
            );
        }
        catch (JsonException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("JSON_DESERIALIZATION_FAILED")
                    .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? Array.Empty<string>())
                        .ToList()) // TODO Splitting the path at '.' is wrong in general.
                    .SetMessage(
                        $"Failed to deserialize GraphQL response of request to the metabase GraphQl endpoint. The details given are: Zero-based number of bytes read within the current line before the exception are {e.BytePositionInLine}, zero-based number of lines read before the exception are {e.LineNumber}, message that describes the current exception is '{e.Message}', path within the JSON where the exception was encountered is {e.Path}.")
                    .SetException(e)
                    .Build()
            );
        }
    }

    private sealed record DatabasesData(DatabasesConnection Databases);
}