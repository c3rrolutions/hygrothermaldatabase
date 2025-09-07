using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.ApiRequests.Dto;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.MethodAsService;

[ExtendObjectType(nameof(Query))]
public sealed class MethodAsServiceQueries
{
    private sealed record DataXResponse(DataXDto DataX);

    public async Task<MethodAsServicePayload> MethodAsServiceWithFileAsync(
        MethodAsServiceWithFileInput input,
        MethodFactory methodFactory,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(input.MethodId);
        if (method is null)
        {
            return new MethodAsServicePayload(
                new MethodAsServiceError(
                    MethodAsServiceErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(input), nameof(input.MethodId).ToLowerFirst()]
                )
            );
        }
        await using var stream = input.File.OpenReadStream();
        var fileInput = await JsonSerializer.DeserializeAsync<FileInputData>(
            stream,
            JsonSerializerSettings.File,
            cancellationToken
            ) ?? throw new JsonException("Failed to deserialize the GraphQL response.");
        var result = method.Calculate(fileInput.Data.DataPoints);
        return new MethodAsServicePayload(result);
    }

    public async Task<MethodAsServicePayload> MethodAsServiceAsync(
        AppSettings appSettings,
        MethodAsServiceInput input,
        MethodFactory methodFactory,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken
    )
    {
        var method = methodFactory.GetMethod(input.MethodId);
        if (method is null)
        {
            return new MethodAsServicePayload(
                new MethodAsServiceError(
                    MethodAsServiceErrorCode.UNKNOWN_METHOD,
                    $"The method is unknown.",
                    [nameof(input), nameof(input.MethodId).ToLowerFirst()]
                )
            );
        }
        DatabaseDto? database;
        try
        {
            database = await DatabaseApi.RequestDatabase(
                input.DatabaseId,
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
            return new MethodAsServicePayload(
                new MethodAsServiceError(
                    MethodAsServiceErrorCode.UNKNOWN_DATABASE,
                    $"The database is unknown.",
                    [nameof(input), nameof(input.DatabaseId).ToLowerFirst()]
                )
            );
        }

        var data = await DataApi.GetDataFromDatabase<DataXResponse>(database.Locator, input.DataId, DataApi.DataXFileNames, appSettings, apiRequestService, httpClientFactory, httpContextAccessor, cancellationToken);

        var locator = data.Data.DataX.ResourceTree.Root.Value.Locator;

        var filedata = await apiRequestService.QueryRest<FileInputData>(new Uri(locator), httpClientFactory, httpContextAccessor, cancellationToken);

        var result = methodFactory.Calculate(filedata.Data.DataPoints.ToList());

        return new MethodAsServicePayload(result);
    }
}