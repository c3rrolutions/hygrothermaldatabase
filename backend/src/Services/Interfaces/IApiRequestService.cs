using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.Services.Interfaces;

/// <summary>
/// Service to request Metabase or Database Api. GraphQL and REST.
/// </summary>
public interface IApiRequestService
{
    /// <summary>
    /// Use Metabase as target for following requests. See https://chillicream.com/docs/hotchocolate/v14/server/dependency-injection
    /// </summary>
    /// <returns> <see cref="IApiRequestService"/> </returns>
    public ApiRequestService Metabase();

    /// <summary>
    /// Use Database as target for following requests. See https://chillicream.com/docs/hotchocolate/v14/server/dependency-injection
    /// </summary>
    /// <returns> <see cref="IApiRequestService"/> </returns>
    public ApiRequestService Database();

    /// <summary>
    /// Construct query from passed files.
    /// </summary>
    /// <param name="fileNames"> Name of files containung queries. </param>
    /// <returns> Query from all files. </returns>
    public Task<string> ConstructGraphQlQuery(string[] fileNames);

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
        AppSettings appSettings,
        GraphQLRequest request,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TGraphQlResponse : class;

    /// <summary>
    /// Send GraphQL request to API and return response.
    /// </summary>
    /// <typeparam name="TGraphQlResponse"> Expected response type. </typeparam>
    /// <param name="appSettings">         <see cref="AppSettings"/> </param>
    /// <param name="uri">                 Uri of GraphQL Api. </param>
    /// <param name="request">             <see cref="GraphQLRequest"/> </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> <see cref="GraphQLResponse{T}"/> of expected type. </returns>
    public Task<GraphQLResponse<TGraphQlResponse>> QueryGraphQlFromUrl<TGraphQlResponse>(
        AppSettings appSettings,
        Uri uri,
        GraphQLRequest request,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TGraphQlResponse : class;

    /// <summary>
    /// Send REST request to API and return response.
    /// </summary>
    /// <typeparam name="TResponse"> Expected response type. </typeparam>
    /// <param name="uri">                 <see cref="Uri"/> to send request to. </param>
    /// <param name="httpClientFactory">   <see cref="IHttpClientFactory"/> </param>
    /// <param name="httpContextAccessor"> <see cref="IHttpContextAccessor"/> </param>
    /// <param name="cancellationToken">   <see cref="CancellationToken"/> </param>
    /// <returns> </returns>
    public Task<TResponse> QueryRest<TResponse>(
        Uri uri,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TResponse : class;
}