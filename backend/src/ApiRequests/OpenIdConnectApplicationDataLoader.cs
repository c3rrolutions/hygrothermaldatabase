using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using HotChocolate;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class OpenIdConnectApplicationDataLoader
{
    private const string QueryFileName = "OpenIdConnectApplications.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record OpenIdConnectApplication(
        [property: GraphQLIgnore] string Id,
        string? Name
    ) : IIdNode<string>
    {
        string ClientId => Id;
    }

    private sealed record OpenIdConnectApplicationsData(
        Connection<OpenIdConnectApplication>? Connection
    ) : IConnectionData<OpenIdConnectApplication>;

    [DataLoader]
    public static Task<Dictionary<string, OpenIdConnectApplication>> GetOpenIdConnectApplicationByClientIdAsync(
        IReadOnlyList<string> clientIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<string, OpenIdConnectApplicationsData, OpenIdConnectApplication>(
            clientIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}