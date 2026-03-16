using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authentication;
using Database.Extensions;
using Database.Json;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Database.Services;

/// <summary>
/// Service to request GraphQL and REST APIs.
/// </summary>
public sealed class ApiRequestService(
    IHttpClientFactory httpClientFactory,
    IHttpContextAccessor httpContextAccessor,
    AppSettings appSettings
)
{
    public const string CustomHttpClient = "Custom";

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
                    JsonDocumentSettings.Strict
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
                    JsonDocumentSettings.Lax
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
        var bearerToken = httpContextAccessor.HttpContext?.ExtractBearerToken();
        using var httpRequestMessage = new HttpRequestMessage(
            httpMethod,
            uri
        );
        httpRequestMessage.Headers.Add(
            HeaderNames.Origin,
            appSettings.Uri.AbsoluteUri
        );
        if (bearerToken is not null)
        {
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(
                OpenIdConnectConstants.AuthorizationHeaderBearer,
                bearerToken
            );
        }
        httpRequestMessage.Content = httpContent;
        using var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        if (httpResponseMessage.StatusCode is not HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"The status code is not {HttpStatusCode.OK} but {httpResponseMessage.StatusCode} and the response is: {httpResponseMessage.Content.ReadAsStringAsync(cancellationToken)}",
                null,
                httpResponseMessage.StatusCode
            );
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
            new System.Net.Http.Headers.MediaTypeHeaderValue(MediaTypeNames.Application.Json);
        return result;
    }
}