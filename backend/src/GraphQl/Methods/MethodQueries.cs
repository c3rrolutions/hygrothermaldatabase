using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Enumerations;
using Database.Json;
using Database.Services;
using GraphQL;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace Database.GraphQl.Methods;

[SuppressMessage("Naming", "CA1707")]
public enum CalculateMethodErrorCode
{
    UNKNOWN_METHOD,
    UNKNOWN_DATABASE,
    DATA_QUERY_FAILED
}

public sealed record CalculateMethodError(
    CalculateMethodErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CalculateMethodErrorCode>(Code, Message, Path);

public sealed record CalculateMethodPayload(
    JsonElement? Result,
    IReadOnlyCollection<CalculateMethodError>? Errors
) : Payload;

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
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )]
            );
        }
        using var stream = data.OpenReadStream();
        using var jsonData = await JsonDocument.ParseAsync(
            stream,
            JsonDocumentSettings.Lax,
            cancellationToken
        );
        var result = method.Calculate(jsonData.RootElement);
        return new CalculateMethodPayload(result, null);
    }

    public async Task<CalculateMethodPayload> CalculateMethodAsync(
        Guid methodId,
        CrossDatabaseDataReferenceInput dataReference,
        AppSettings appSettings,
        MethodFactory methodFactory,
        ApiRequestService apiRequestService,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(methodId);
        if (method is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(methodId)]
                )]
            );
        }
        var database = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => QueryDatabase.Do(
                dataReference.DatabaseId,
                appSettings,
                apiRequestService,
                cancellationToken
            ),
            resolverContext,
            QueryDatabase.GetGraphQlEndpoint(appSettings)
        );
        if (database is null)
        {
            return new CalculateMethodPayload(
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.UNKNOWN_DATABASE,
                    $"The database is unknown.",
                    [nameof(dataReference), nameof(dataReference.DatabaseId).ToLowerFirst()]
                )]
            );
        }
        var query = ConstructQuery(dataReference.DataKind);
        GraphQLResponse<DataData>? response;
        response = await GraphQlRequestHelper.TransformExceptionsAsync(
            () => QueryData.Do<DataData>(
                database.Locator,
                dataReference.DataId,
                query,
                apiRequestService,
                cancellationToken
            ),
            resolverContext,
            database.Locator
        );
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
                null,
                [new CalculateMethodError(
                    CalculateMethodErrorCode.DATA_QUERY_FAILED,
                    $"Failed to query database {database.Locator} for the data.",
                    [nameof(dataReference)]
                )]
            );
        }
        var locator = response.Data.Data.ResourceTree.Root.Value.Locator;
        // TODO The locator could also point to a non-JSON resource. Support those also: response.Data.Data.ResourceTree.Root.Value.DataFormatId
        // TODO Check that the data has the hash value: response.Data.Data.ResourceTree.Root.Value.HashValue;
        var data = await apiRequestService.PerformHttpGetRequest(
            locator, cancellationToken
        );
        var result = method.Calculate(data);
        return new CalculateMethodPayload(result, null);
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