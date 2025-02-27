using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Database.Extensions;
using GraphQL;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.AspNetCore.Http;

namespace Database.Metabase;

public sealed class QueryingMetabase
{
    public const string MetabaseHttpClient = "Metabase";

    private static readonly JsonSerializerOptions GraphQlSerializerOptions =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();

    private static readonly JsonSerializerOptions RestSerializerOptions =
        new()
        {
            Converters = { new JsonStringEnumConverter(new ConstantCaseJsonNamingPolicy(), false) },
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReadCommentHandling = JsonCommentHandling.Disallow,
            IncludeFields = false,
            IgnoreReadOnlyProperties = false,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }; //.SetupImmutableConverter();

    public static async Task<string> ConstructGraphQlQuery(
        string[] fileNames
    )
    {
        return string.Join(
            Environment.NewLine,
            await Task.WhenAll(
                fileNames.Select(fileName =>
                    File.ReadAllTextAsync($"Metabase/Queries/{fileName}")
                )
            ).ConfigureAwait(false)
        );
    }

    public static Task<GraphQLResponse<TGraphQlResponse>> QueryGraphQl<TGraphQlResponse>(
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
            // TODO Consider using [Flurl](https://flurl.dev) to construct URIs. For the pitfalls of using `Uri` as below see the comments to https://stackoverflow.com/questions/372865/path-combine-for-urls/1527643#1527643
            new Uri(new Uri(appSettings.MetabaseHost), "/graphql/"),
            MakeJsonHttpContent(request),
            GraphQlSerializerOptions,
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        );
    }

    private static ByteArrayContent MakeJsonHttpContent<TContent>(
        TContent content
    )
    {
        // For some reason using `JsonContent.Create<TContent>(content, null, SerializerOptions)` results in status code `BadRequest`.
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    GraphQlSerializerOptions
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue("application/json");
        return result;
    }

    public static Task<TResponse> QueryRest<TResponse>(
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
            RestSerializerOptions,
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
        using var httpClient = httpClientFactory.CreateClient(MetabaseHttpClient);
        var bearerToken = await httpContextAccessor.ExtractBearerToken().ConfigureAwait(false);
        using var httpRequestMessage = new HttpRequestMessage(
            httpMethod,
            uri
        );
        httpRequestMessage.Content = httpContent;
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        using var httpResponseMessage =
            await httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            throw new HttpRequestException(
                $"The status code is not {HttpStatusCode.OK} but {httpResponseMessage.StatusCode}.", null,
                httpResponseMessage.StatusCode);
        // We could use `httpResponseMessage.Content.ReadFromJsonAsync<GraphQL.GraphQLResponse<TResponse>>` which would make debugging more difficult though, https://docs.microsoft.com/en-us/dotnet/api/system.net.http.json.httpcontentjsonextensions.readfromjsonasync?view=net-5.0#System_Net_Http_Json_HttpContentJsonExtensions_ReadFromJsonAsync__1_System_Net_Http_HttpContent_System_Text_Json_JsonSerializerOptions_System_Threading_CancellationToken_
        using var responseStream =
            await httpResponseMessage.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
        // For debugging, the following lines of code write the response to standard output.
        // Console.WriteLine(new StreamReader(responseStream).ReadToEnd());
        var deserializedResponse =
            await JsonSerializer.DeserializeAsync<TResponse>(
                responseStream,
                serializerOptions,
                cancellationToken
            ).ConfigureAwait(false);
        if (deserializedResponse is null) throw new JsonException("Failed to deserialize the GraphQL response.");
        return deserializedResponse;
    }
}