using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests.Dto;
using Database.GraphQl.Databases;
using Database.Services;
using GraphQL;
using HotChocolate;
using Microsoft.AspNetCore.Http;

namespace Database.ApiRequests;

public sealed class DatabaseApi
{
    private static readonly string[] _databaseFileNames =
    [
        "Database.graphql"
    ];

    private static readonly string[] _updateDatabaseFileNames =
    [
        "UpdateDatabase.graphql"
    ];

    private sealed record DatabaseData(DatabaseDto Database);

    public static async Task<DatabaseDto?> RequestDatabase(
        Guid databaseId,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var response = await apiRequestService.Metabase().QueryGraphQl<DatabaseData>(
                    appSettings,
                    new GraphQLRequest(await apiRequestService.ConstructGraphQlQuery(_databaseFileNames),
                        new
                        {
                            id = databaseId
                        },
                        "Database"
                    ),
                    httpClientFactory,
                    httpContextAccessor,
                    cancellationToken
                );
        return response.Data?.Database;
    }

    public static async Task<DatabasePayloadDto?> UpdateDatabase(
        UpdateDatabaseInput updateDatabaseInput,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
        )
    {
        return (await apiRequestService.Metabase().QueryGraphQl<DatabasePayloadDto>(
            appSettings,
            new GraphQLRequest(
                await apiRequestService.ConstructGraphQlQuery(
                    _updateDatabaseFileNames
                    ),
                new
                {
                    input = updateDatabaseInput
                },
                "UpdateDatabase"
                ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
            ))?.Data;
    }
}