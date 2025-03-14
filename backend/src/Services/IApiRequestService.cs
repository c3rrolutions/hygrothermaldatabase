using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.AspNetCore.Http;

namespace Database.Services;

public interface IApiRequestService
{
    public ApiRequestService Metabase();

    public ApiRequestService Database();

    public Task<string> ConstructGraphQlQuery(string[] fileNames);

    public Task<GraphQLResponse<TGraphQlResponse>> QueryGraphQl<TGraphQlResponse>(
        AppSettings appSettings,
        GraphQLRequest request,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TGraphQlResponse : class;

    public Task<TResponse> QueryRest<TResponse>(
        Uri uri,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
        where TResponse : class;
}