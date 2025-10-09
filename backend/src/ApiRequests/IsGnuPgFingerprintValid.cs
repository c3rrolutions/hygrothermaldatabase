using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Database.Services;

namespace Database.ApiRequests;

public sealed class IsGnuPgFingerprintValid
{
    private const string QueryFileName = "IsGnuPgFingerprintValid.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        ApiRequestService.MetabaseGraphQlEndpoint(appSettings);

    public sealed record ValidFingerprints(
        uint TotalCount
    );

    private sealed record Data(ValidFingerprints? ValidFingerprints);

    public static async Task<bool> Do(
        string fingerprint,
        Guid institutionId,
        DateTime createdAt,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.Metabase().QueryGraphQl<Data>(
            appSettings,
            new GraphQLRequest(
                await apiRequestService.ConstructGraphQlQuery(QueryFileName),
                new
                {
                    fingerprint,
                    institutionId,
                    createdAt
                },
                "IsGnuPgFingerprintValid"
            ),
            httpClientFactory,
            httpContextAccessor,
            cancellationToken
        )).Data.ValidFingerprints?.TotalCount == 1;
    }
}