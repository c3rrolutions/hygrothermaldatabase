using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Database.Data;
using Database.Json;
using FluentAssertions;
using Json.Path;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Snapshooter;

namespace Database.Tests.Integration;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract partial class IntegrationTests
    : IDisposable
{
    private bool _disposed;

    protected IntegrationTests()
    {
        Factory = new CustomWebApplicationFactory();
        HttpClient = CreateHttpClient();
    }

    private CustomWebApplicationFactory Factory { get; }

    protected CollectingEmailSender EmailSender => Factory.EmailSender;

    protected AppSettings AppSettings => Factory.AppSettings;

    protected HttpClient HttpClient { get; }

    public Task DoAsync(Func<ApplicationDbContext, Task> what)
    {
        return Factory.DoAsync(what);
    }

    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);
        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    // https://docs.microsoft.com/en-us/dotnet/standard/managed-code
    // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
    ~IntegrationTests()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Factory.Dispose();
                HttpClient.Dispose();
            }

            _disposed = true;
        }
    }

    private static IEnumerable<Cookie> ExtractCookies(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var cookieEntries))
        {
            return [];
        }
        var uri = response.RequestMessage?.RequestUri ?? throw new ArgumentException($"The request URI cannot be extracted from the given response {response}.");
        var cookieContainer = new CookieContainer();
        foreach (var cookieEntry in cookieEntries)
        {
            cookieContainer.SetCookies(uri, cookieEntry);
        }
        return cookieContainer.GetCookies(uri).Cast<Cookie>();
    }

    private static async Task UpdateAntiforgeryCookieAndToken(
        HttpClient httpClient
    )
    {
        // Get the antiforgery token in the cookie "XSRF-TOKEN" and set it
        // permanently on the HTTP client by requesting /antiforgery/token
        // synchronously.
        var response = await httpClient.GetAsync("/antiforgery/token");
        // Add the antiforgery token as the default request header "X-XSRF-TOKEN".
        var xsrfToken =
            ExtractCookies(response)
            .SingleOrDefault(cookie => cookie.Name == "XSRF-TOKEN")
            ?.Value
            ?? throw new ArgumentException("The `XSRF-TOKEN` cookie is missing in the response of a request to /antiforgery/token");
        httpClient.DefaultRequestHeaders.Remove("X-XSRF-TOKEN");
        httpClient.DefaultRequestHeaders.Add("X-XSRF-TOKEN", xsrfToken);
    }

    protected static HttpClient CreateHttpClient(
        CustomWebApplicationFactory factory,
        bool allowAutoRedirect = true
    )
    {
        var httpClient = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = allowAutoRedirect,
                BaseAddress = new Uri("http://localhost", UriKind.Absolute),
                HandleCookies = true,
                MaxAutomaticRedirections = 3
            }
        );
        // TODO Why does running `Task.Run(() => UpdateAntiforgeryCookieAndToken(httpClient)).Wait();` here fails with "cookie is missing" here? It works in the metabase.
        return httpClient;
    }

    protected HttpClient CreateHttpClient(
        bool allowAutoRedirect = true
    )
    {
        return CreateHttpClient(
            Factory,
            allowAutoRedirect
        );
    }

    protected Task<HttpResponseMessage> QueryGraphQl(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return QueryGraphQl(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static Task<HttpResponseMessage> QueryGraphQl(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return httpClient.PostAsync(
            "/graphql",
            MakeJsonHttpContent(
                new GraphQlRequest(
                    query,
                    operationName,
                    variables
                )
            )
        );
    }

    protected Task<HttpContent> SuccessfullyQueryGraphQlContent(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContent(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<HttpContent> SuccessfullyQueryGraphQlContent(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        var httpResponseMessage = await QueryGraphQl(
            httpClient,
            query,
            operationName,
            variables
        );
        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
        {
            // We wrap this check in an if-condition such that the message
            // content is only read when the status code is not 200.
            httpResponseMessage.StatusCode.Should().Be(
                HttpStatusCode.OK,
                await httpResponseMessage.Content.ReadAsStringAsync()
            );
        }

        return httpResponseMessage.Content;
    }

    protected Task<HttpContent> UnsuccessfullyQueryGraphQlContent(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return UnsuccessfullyQueryGraphQlContent(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<HttpContent> UnsuccessfullyQueryGraphQlContent(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        var httpResponseMessage = await QueryGraphQl(
            httpClient,
            query,
            operationName,
            variables
        );
        if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        {
            // We wrap this check in an if-condition such that the message
            // content is only read when the status code is not 200.
            httpResponseMessage.StatusCode.Should().NotBe(
                HttpStatusCode.OK,
                await httpResponseMessage.Content.ReadAsStringAsync()
            );
        }

        return httpResponseMessage.Content;
    }

    protected Task<string> SuccessfullyQueryGraphQlContentAsString(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContentAsString(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<string> SuccessfullyQueryGraphQlContentAsString(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return await (
                await SuccessfullyQueryGraphQlContent(
                        httpClient,
                        query,
                        operationName,
                        variables
                    )
            )
            .ReadAsStringAsync();
    }

    protected Task<JsonElement> SuccessfullyQueryGraphQlContentAsJson(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return SuccessfullyQueryGraphQlContentAsJson(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<JsonElement> SuccessfullyQueryGraphQlContentAsJson(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        using var document = await JsonDocument.ParseAsync(
            await (
                    await SuccessfullyQueryGraphQlContent(
                            httpClient,
                            query,
                            operationName,
                            variables
                        )
                )
                .ReadAsStreamAsync()
        );
        return document.RootElement.Clone();
    }

    protected Task<string> UnsuccessfullyQueryGraphQlContentAsString(
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return UnsuccessfullyQueryGraphQlContentAsString(
            HttpClient,
            query,
            operationName,
            variables
        );
    }

    protected static async Task<string> UnsuccessfullyQueryGraphQlContentAsString(
        HttpClient httpClient,
        string query,
        string? operationName = null,
        object? variables = null
    )
    {
        return await (
                await UnsuccessfullyQueryGraphQlContent(
                        httpClient,
                        query,
                        operationName,
                        variables
                    )
            )
            .ReadAsStringAsync();
    }

    protected static string ExtractString(
        string jsonPath,
        JsonElement jsonElement
    )
    {
        var pathResult =
            JsonPath.Parse(jsonPath).Evaluate(
                JsonObject.Create(jsonElement)
            );

        return pathResult.Matches?.Single()?.Value?.GetValue<string>()
               ?? throw new ArgumentException("String is null");
    }

    protected static Guid ExtractUuid(
        string jsonPath,
        JsonElement jsonElement
    )
    {
        return new Guid(
            ExtractString(
                jsonPath,
                jsonElement
            )
        );
    }

    protected static string Base64Encode(string text)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(text)
        );
    }

    protected static string Base64Decode(string text)
    {
        return Encoding.UTF8.GetString(
            Convert.FromBase64String(text)
        );
    }

    protected void EmailsShouldContainSingle(
        (string name, string address) recipient,
        string subject,
        string bodyRegEx
    )
    {
        EmailSender.Emails.Should().ContainSingle();
        var email = EmailSender.Emails.First();
        email.Recipient.Should().Be(recipient);
        email.Subject.Should().Be(subject);
        email.Body.Should().MatchRegex(bodyRegEx);
    }

    private static ByteArrayContent MakeJsonHttpContent<TContent>(
        TContent content
    )
    {
        var result =
            new ByteArrayContent(
                JsonSerializer.SerializeToUtf8Bytes(
                    content,
                    JsonSerializerSettings.GraphQl
                )
            );
        result.Headers.ContentType =
            new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
        return result;
    }

    // With NUnit using async Snapshooter is not able to calculate
    // the necessary Fullname, due to reasons mentioned in
    // https://stackoverflow.com/questions/22598323/movenext-instead-of-actual-method-task-name
    // The workaround with optional parameters is inspired by the same source.
    protected static SnapshotFullName SnapshotFullNameHelper(
        Type testType,
        string keyName,
        [CallerMemberName] string testMethod = "",
        [CallerFilePath] string testFilePath = ""
    )
    {
        var testName = $"{testType.Name}.{testMethod}_{keyName}.snap";
        var testDirectory =
            Path.GetDirectoryName(testFilePath)
            ?? throw new ArgumentException($"The path '{testFilePath}' denotes a root directory or is `null`.");
        return new SnapshotFullName(testName, testDirectory);
    }

    private sealed class GraphQlRequest(
        string query,
        string? operationName,
        object? variables
        )
    {
        public string Query { get; } = query;
        public string? OperationName { get; } = operationName;
        public object? Variables { get; } = variables;
    }
}