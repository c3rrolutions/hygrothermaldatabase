using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;

namespace Database.ApiRequests;

public static class MethodDataLoader
{
    private const string QueryFileName = "Methods.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Method(
        Guid Uuid
    ) : IIdNode;

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
        return QueryByIdDataLoader.GetByIdAsync<MethodsData, Method>(
            methodIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}