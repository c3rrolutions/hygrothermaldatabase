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
/// Implementation of <see cref="IApiRequestService"/>
/// </summary>
public class ApiRequestService() : IApiRequestService
{
    /// <summary>
    /// Name of Metabase http client.
    /// </summary>
    public const string MetabaseHttpClient = "Metabase";

    /// <summary>
    /// Name of Database http client.
    /// </summary>
    public const string DatabaseHttpClient = "Database";

    private static bool _useMetabase = true;

    /// <inheritdoc/>
    public ApiRequestService Metabase()
    {
        _useMetabase = true;
        return this;
    }

    /// <inheritdoc/>
    public ApiRequestService Database()
    {
        _useMetabase = false;
        return this;
    }

    /// <inheritdoc/>
    public async Task<string> ConstructGraphQlQuery(
        string[] fileNames
    )
    {
        return string.Join(
            Environment.NewLine,
            await Task.WhenAll(
                fileNames.Select(fileName =>
                    File.ReadAllTextAsync($"ApiRequests/Queries/{fileName}")
                )
            ).ConfigureAwait(false)
        );
    }

    /// <inheritdoc/>
    public Task<GraphQLResponse<TGraphQlResponse>> QueryGraphQl<TGraphQlResponse>(
        AppSettings appSettings,
        GraphQLRequest request,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TGraphQlResponse : class
    {
        return Query<GraphQLResponse<TGraphQlResponse>>(
            HttpMethod.Post,
            // TODO Consider using [Flurl](https://flurl.dev) to construct URIs. For the pitfalls of
            // using `Uri` as below see the comments to https://stackoverflow.com/questions/372865/path-combine-for-urls/1527643#1527643
            new Uri(new Uri(_useMetabase ? appSettings.MetabaseHost : appSettings.Host), "/graphql/"),
            MakeJsonHttpContent(request),
            JsonSerializerSettings.GraphQL,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public Task<TResponse> QueryRest<TResponse>(
        Uri uri,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        return Query<TResponse>(
            HttpMethod.Get,
            uri,
            null,
            JsonSerializerSettings.Rest,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }

    private static async Task<TResponse> Query<TResponse>(
        HttpMethod httpMethod,
        Uri uri,
        HttpContent? httpContent,
        JsonSerializerOptions serializerOptions,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        using var httpClient = _useMetabase ? httpClientFactory.CreateClient(MetabaseHttpClient) : httpClientFactory.CreateClient(DatabaseHttpClient);
        var bearerToken = await httpContextAccessor.ExtractBearerToken().ConfigureAwait(false);
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
            await httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            throw new HttpRequestException(
                $"The status code is not {HttpStatusCode.OK} but {httpResponseMessage.StatusCode}.", null,
                httpResponseMessage.StatusCode);

        // We could use
        // `httpResponseMessage.Content.ReadFromJsonAsync<GraphQL.GraphQLResponse<TResponse>>` which
        // would make debugging more difficult though, https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync?view=net-5.0#System_Net_Http_Json_HttpContentJsonExtensions_ReadFromJsonAsync__1_System_Net_Http_HttpContent_System_Text_Json_JsonSerializerOptions_System_Threading_CancellationToken_
        using var responseStream = await httpResponseMessage.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        // For debugging, the following lines of code write the response to standard output.
        // Console.WriteLine(new StreamReader(responseStream).ReadToEnd());
        var deserializedResponse = await JsonSerializer.DeserializeAsync<TResponse>(
            responseStream,
            serializerOptions,
            cancellationToken
            ).ConfigureAwait(false);

        if (deserializedResponse is null) throw new JsonException("Failed to deserialize the GraphQL response.");

        return deserializedResponse;
    }

    private static ByteArrayContent MakeJsonHttpContent<TContent>(
        TContent content
    )
    {
        // For some reason using `JsonContent.Create<TContent>(content, null, SerializerOptions)`
        // results in status code `BadRequest`.
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    JsonSerializerSettings.GraphQL
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return result;
    }
}