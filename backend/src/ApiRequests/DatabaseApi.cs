using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.GraphQl.Databases;
using Database.Services.Interfaces;
using GraphQL;
using HotChocolate;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

/// <summary>
/// Class to request databases from Metabase API.
/// </summary>
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

    private sealed record DatabasesData(DatabasesConnectionDto Databases);

    /// <summary>
    /// Request database from Metabase.
    /// </summary>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="resolverContext">     <see cref="IResolverContext"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="DatabaseDto"/> if successful. </returns>
    /// <exception cref="GraphQLException"> Throws exception if errors occur. </exception>
    public static async Task<DatabaseDto> RequestDatabase(
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken)
    {
        var response = await apiRequestService.Metabase().QueryGraphQl<DatabasesData>(
                    appSettings,
                    new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(_databaseFileNames).ConfigureAwait(false),
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

    /// <summary>
    /// Request database from Metabase.
    /// </summary>
    /// <param name="databaseId">          Id of database. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="resolverContext">     <see cref="IResolverContext"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="DatabaseDto"/> if successful. </returns>
    /// <exception cref="GraphQLException"> Throws exception if errors occur. </exception>
    public static async Task<DatabaseDto> RequestDatabaseById(
        Guid databaseId,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken)
    {
        var response = await apiRequestService.Metabase().QueryGraphQl<DatabasesData>(
                    appSettings,
                    new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(_databaseFileNames).ConfigureAwait(false),
                        new
                        {
                            where = new
                            {
                                uuid = new
                                {
                                    equalTo = databaseId.ToString(),
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

    /// <summary>
    /// Update database in Metabase.
    /// </summary>
    /// <param name="updateDatabaseInput"> <see cref="UpdateDatabaseInput"/> with update mutation. </param>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="apiRequestService">   <see cref="IApiRequestService"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> Updated <see cref="DatabaseDto"/> </returns>
    public static async Task<DatabaseDto?> UpdateDatabase(
        UpdateDatabaseInput updateDatabaseInput,
        AppSettings appSettings,
        IApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        return (await apiRequestService.Metabase().QueryGraphQl<DatabaseDto>(
            appSettings,
            new GraphQLRequest(
                await apiRequestService.ConstructGraphQlQuery(
                    _updateDatabaseFileNames
                    ).ConfigureAwait(false),
                new
                {
                    updateDatabaseInput
                },
                "UpdateDatabase"
                ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ).ConfigureAwait(false))?.Data;
    }
}