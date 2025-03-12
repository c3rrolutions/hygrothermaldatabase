using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequest.Dto;
using Database.GraphQl.Databases;
using GraphQL;
using HotChocolate;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequest;

public class DatabaseApi
{
    private static readonly string[] _databaseFileNames =
    {
        "Databases.graphql"
    };

    private static readonly string[] _updateDatabaseFileNames =
    {
        "UpdateDatabase.graphql"
    };

    private sealed record DatabasesData(DatabasesConnection Databases);

    public static async Task<DatabaseDto> RequestDatabase(
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken)
    {
        var response = await ApiRequestService.QueryGraphQl<DatabasesData>(
                    appSettings,
                    new GraphQLRequest(
                        await ApiRequestService.ConstructGraphQlQuery(
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

    public static async Task<DatabaseDto?> UpdateDatabase(
        DatabaseRequestDto databaseRequest,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        return (await ApiRequestService.QueryGraphQl<DatabaseDto>(
            appSettings,
            new GraphQLRequest(
                await ApiRequestService.ConstructGraphQlQuery(
                    _updateDatabaseFileNames
                    ).ConfigureAwait(false),
                new
                {
                    databaseRequest
                },
                "UpdateDatabase"
                ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false))?.Data;
    }
}