using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using HotChocolate;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class InstitutionDataLoader
{
    private const string QueryFileName = "Institutions.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Institution(
        [property: GraphQLIgnore] Guid Id,
        string Name
    ) : IIdNode<Guid>
    {
        public Guid Uuid => Id;
    }

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
        return QueryByIdDataLoader.GetByIdAsync<Guid, InstitutionsData, Institution>(
            institutionIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}