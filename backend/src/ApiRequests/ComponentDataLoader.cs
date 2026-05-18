using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class ComponentDataLoader
{
    private const string QueryFileName = "Components.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Component(
        Guid Id
    ) : IIdNode<Guid>;

    private sealed record ComponentsData(
        Connection<Component>? Connection
    ) : IConnectionData<Component>;

    [DataLoader]
    public static Task<Dictionary<Guid, Component>> GetComponentByIdAsync(
        IReadOnlyList<Guid> componentIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<Guid, ComponentsData, Component>(
            componentIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}