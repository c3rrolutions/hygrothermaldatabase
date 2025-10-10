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
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record Institution(
        bool HasGnuPgKeyFingerprint
    );

    private sealed record Data(Institution? Institution);

    public static async Task<bool> Do(
        string fingerprint,
        Guid institutionId,
        DateTime createdAt,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken
    )
    {
        return (await apiRequestService.QueryGraphQl<Data>(
            GetGraphQlEndpoint(appSettings),
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
            cancellationToken
        )).Data.Institution?.HasGnuPgKeyFingerprint ?? false;
    }
}