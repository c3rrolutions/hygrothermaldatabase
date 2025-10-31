using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;

namespace Database.ApiRequests;

public static class InstitutionDataLoader
{
    private const string QueryFileName = "Institutions.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Institution(
        Guid Uuid
    ) : IIdNode;

    private sealed record InstitutionsData(
        Connection<Institution>? Connection
    ) : IConnectionData<Institution>;

    [DataLoader]
    public static Task<Dictionary<Guid, Institution>> GetInstitutionByIdAsync(
        IReadOnlyList<Guid> institutionIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<InstitutionsData, Institution>(
            institutionIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}