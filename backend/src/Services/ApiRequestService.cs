using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Extensions;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

/// <summary>
/// Service to request GraphQL and REST APIs.
/// </summary>
public sealed class ApiRequestService(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor
)
{
    public const string CustomHttpClient = "Custom";

    public static JsonDocumentOptions StrictJsonDocumentOptions => new()
    {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 0
    };

    public static JsonDocumentOptions LaxJsonDocumentOptions => new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        MaxDepth = 0
    };

    public Task<string> ConstructGraphQlQuery(
        string fileName
    )
    {
        return ConstructGraphQlQuery([fileName]);
    }

    /// <summary>
    /// Construct query from passed files.
    /// </summary>
    /// <param name="fileNames"> Name of files containung queries. </param>
    /// <returns> Query from all files. </returns>
    public async Task<string> ConstructGraphQlQuery(
        string[] fileNames
    )
    {
        return string.Join(
            Environment.NewLine,
            await Task.WhenAll(
                fileNames.Select(fileName =>
                    File.ReadAllTextAsync($"./ApiRequests/Queries/{fileName}")
                )
            )
        );
    }

    public Task<JsonElement> QueryGraphQlAsJson(
        Uri url,
        GraphQLRequest request,
        CancellationToken cancellationToken
    )
    {
        return Query(
            HttpMethod.Post,
            url,
            async httpResponseContent =>
            {
                using var document = await JsonDocument.ParseAsync(
                    await httpResponseContent.ReadAsStreamAsync(),
                    StrictJsonDocumentOptions
                );
                return document.RootElement.Clone();
            },
            MakeGraphQlJsonHttpContent(request),
            cancellationToken
        );
    }

    public Task<string> QueryGraphQlAsString(
        Uri url,
        GraphQLRequest request,
        CancellationToken cancellationToken
    )
    {
        return Query(
            HttpMethod.Post,
            url,
            httpResponseContent => httpResponseContent.ReadAsStringAsync(cancellationToken),
            MakeGraphQlJsonHttpContent(request),
            cancellationToken
        );
    }

    /// <summary>
    /// Send GraphQL request to API and return response.
    /// </summary>
    /// <typeparam name="TGraphQlResponse"> Expected response type. </typeparam>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="request">             <see cref="GraphQLRequest"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="GraphQLResponse{T}"/> of expected type. </returns>
    public Task<GraphQLResponse<TGraphQlResponse>> QueryGraphQl<TGraphQlResponse>(
        Uri url,
        GraphQLRequest request,
        CancellationToken cancellationToken
    )
        where TGraphQlResponse : class
    {
        return Query<GraphQLResponse<TGraphQlResponse>>(
            HttpMethod.Post,
            url,
            MakeGraphQlJsonHttpContent(request),
            JsonSerializerSettings.GraphQl,
            cancellationToken
        );
    }

    public Task<JsonElement> PerformHttpGetRequest(
        Uri url,
        CancellationToken cancellationToken
    )
    {
        return Query(
            HttpMethod.Get,
            url,
            async httpResponseContent =>
            {
                using var document = await JsonDocument.ParseAsync(
                    await httpResponseContent.ReadAsStreamAsync(),
                    LaxJsonDocumentOptions
                );
                return document.RootElement.Clone();
            },
            null,
            cancellationToken
        );
    }

    public Task<TResponse> PerformHttpGetRequest<TResponse>(
        Uri uri,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        return Query<TResponse>(
            HttpMethod.Get,
            uri,
            null,
            JsonSerializerSettings.Rest,
            cancellationToken
        );
    }

    private async Task<TResponse> Query<TResponse>(
        HttpMethod httpMethod,
        Uri url,
        HttpContent? httpRequestContent,
        JsonSerializerOptions serializerOptions,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        // We could use
        // `httpResponseContent.ReadFromJsonAsync<GraphQL.GraphQLResponse<TResponse>>` which
        // would make debugging more difficult though, https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync?view=net-5.0#System_Net_Http_Json_HttpContentJsonExtensions_ReadFromJsonAsync__1_System_Net_Http_HttpContent_System_Text_Json_JsonSerializerOptions_System_Threading_CancellationToken_
        return await Query(
            httpMethod,
            url,
            async httpResponseContent =>
            {
                var responseStream = await httpResponseContent.ReadAsStreamAsync();
                // For debugging, the following line of code writes the response to standard output.
                // Console.WriteLine(new StreamReader(responseStream).ReadToEnd());
                return await JsonSerializer.DeserializeAsync<TResponse>(
                    responseStream,
                    serializerOptions,
                    cancellationToken
                    ) ?? throw new JsonException("Failed to deserialize the GraphQL response.");

            },
            httpRequestContent,
            cancellationToken
        );
    }

    private async Task<T> Query<T>(
        HttpMethod httpMethod,
        Uri uri,
        Func<HttpContent, Task<T>> read,
        HttpContent? httpContent,
        CancellationToken cancellationToken
    )
    {
        using var httpClient = httpClientFactory.CreateClient(CustomHttpClient);
        var bearerToken = await httpContextAccessor.ExtractBearerToken();
        using var httpRequestMessage = new HttpRequestMessage(
            httpMethod,
            uri
        );
        httpRequestMessage.Content = httpContent;
        if (bearerToken is not null)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        using var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"The status code is not {HttpStatusCode.OK} but {httpResponseMessage.StatusCode}.", null,
                httpResponseMessage.StatusCode);
        }
        return await read(httpResponseMessage.Content);
    }

    private static ByteArrayContent MakeGraphQlJsonHttpContent<TContent>(
        TContent content
    )
    {
        // For some reason using `JsonContent.Create<TContent>(content, null, SerializerOptions)`
        // results in status code `BadRequest`.
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    JsonSerializerSettings.GraphQl
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return result;
    }
}