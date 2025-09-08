using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Enumerations;
using Database.Services;
using GraphQL;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Methods;

[ExtendObjectType(nameof(Query))]
public sealed class MethodQueries
{
    private sealed record DataData(
        Data? Data
    );

    private sealed record Data(
        ResourceTree ResourceTree
    );

    private sealed record ResourceTree(
        Root Root
    );

    private sealed record Root(
        Resource Value
    );

    private sealed record Resource(
        string HashValue,
        Uri Locator,
        Guid DataFormatId
    );

    public async Task<CalculateMethodPayload> CalculateMethodWithDataUploadAsync(
        Guid methodId,
        [GraphQLType(typeof(NonNullType<UploadType>))] IFile data,
        MethodFactory methodFactory,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(methodId);
        if (method is null)
        {
            return new CalculateMethodPayload(
                new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )
            );
        }
        using var stream = data.OpenReadStream();
        using var jsonData = await JsonDocument.ParseAsync(
            stream,
            ApiRequestService.JsonDocumentOptions,
            cancellationToken
        );
        var result = method.Calculate(jsonData.RootElement);
        return new CalculateMethodPayload(result);
    }

    public async Task<CalculateMethodPayload> CalculateMethodAsync(
        Guid methodId,
        CrossDatabaseDataReferenceInput dataReference,
        AppSettings appSettings,
        MethodFactory methodFactory,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(methodId);
        if (method is null)
        {
            return new CalculateMethodPayload(
                new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )
            );
        }
        DatabaseDto? database;
        try
        {
            database = await DatabaseApi.RequestDatabase(
                dataReference.DatabaseId,
                appSettings,
                apiRequestService,
                httpClientFactory,
                httpContextAccessor,
                cancellationToken);
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
                    .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? [])
                        .ToList()) // TODO Splitting the path at '.' is wrong in general.
                    .SetMessage(
                        $"Failed to deserialize GraphQL response of request to the metabase GraphQl endpoint. The details given are: Zero-based number of bytes read within the current line before the exception are {e.BytePositionInLine}, zero-based number of lines read before the exception are {e.LineNumber}, message that describes the current exception is '{e.Message}', path within the JSON where the exception was encountered is {e.Path}.")
                    .SetException(e)
                    .Build()
            );
        }
        if (database is null)
        {
            return new CalculateMethodPayload(
                new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_DATABASE,
                    $"The database is unknown.",
                    [nameof(dataReference), nameof(dataReference.DatabaseId).ToLowerFirst()]
                )
            );
        }
        var query = ConstructQuery(dataReference.DataKind);
        GraphQLResponse<DataData>? response;
        try
        {
            response = await DataApi.QueryDataFromDatabase<DataData>(
                database.Locator,
                dataReference.DataId,
                query,
                appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken
            );
        }
        catch (HttpRequestException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("DATABASE_REQUEST_FAILED")
                    .SetPath(resolverContext.Path)
                    .SetMessage($"Failed with status code {e.StatusCode} to request the database GraphQl endpoint {database.Locator}.")
                    .SetException(e)
                    .Build()
            );
        }
        catch (JsonException e)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetCode("JSON_DESERIALIZATION_FAILED")
                    .SetPath(resolverContext.Path.ToList().Concat(e.Path?.Split('.') ?? [])
                        .ToList()) // TODO Splitting the path at '.' is wrong in general.
                    .SetMessage(
                        $"Failed to deserialize GraphQL response of request to the database GraphQl endpoint {database.Locator}. The details given are: Zero-based number of bytes read within the current line before the exception are {e.BytePositionInLine}, zero-based number of lines read before the exception are {e.LineNumber}, message that describes the current exception is '{e.Message}', path within the JSON where the exception was encountered is {e.Path}.")
                    .SetException(e)
                    .Build()
            );
        }
        if (response.Data?.Data is null)
        {
            if (response.Errors?.Length >= 1)
            {
                foreach (var error in response.Errors)
                {
                    var errorBuilder = ErrorBuilder.New()
                        .SetCode("DATABASE_QUERY_ERROR")
                        // .SetPath(error.Path) // TODO Add the error path. Just using `error.Path` does not work as it contains non-"GraphQlName"s according to HotChocolate sometimes.
                        .SetMessage($"The GraphQL response received from the database {database.Locator} for the query {query} reported the error {error.Message}.");
                    resolverContext.ReportError(errorBuilder.Build());
                }
            }
            return new CalculateMethodPayload(
                new CalculateMethodError(
                    CalculateMethodErrorCode.DATA_QUERY_FAILED,
                    $"Failed to query database {database.Locator} for the data.",
                    [nameof(dataReference)]
                )
            );
        }
        var locator = response.Data.Data.ResourceTree.Root.Value.Locator;
        // TODO The locator could also point to a non-JSON resource. Support those also.
        var data = await apiRequestService.PerformHttpGetRequest(
            locator, httpClientFactory, httpContextAccessor, cancellationToken
        );
        var result = method.Calculate(data);
        return new CalculateMethodPayload(result);
    }

    private static string ConstructQuery(DataKind dataKind)
    {
        var name = Enum.GetName(dataKind)![..^"_DATA".Length].ToLowerInvariant();
        return
            $$"""
                query Data($id: Uuid!) {
                  data: {{name}}Data(id: $id) {
                    resourceTree {
                      root {
                        value {
                          hashValue
                          locator
                          dataFormatId
                        }
                      }
                    }
                  }
                }
            """;
    }
}