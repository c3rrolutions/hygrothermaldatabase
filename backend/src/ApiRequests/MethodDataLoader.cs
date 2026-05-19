using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class MethodDataLoader
{
    private const string QueryFileName = "Methods.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Method(
        Guid Id
    ) : IIdNode<Guid>;

    private sealed record MethodsData(
        Connection<Method>? Connection
    ) : IConnectionData<Method>;

    [DataLoader]
    public static Task<Dictionary<Guid, Method>> GetMethodByIdAsync(
        IReadOnlyList<Guid> methodIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<Guid, MethodsData, Method>(
            methodIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}