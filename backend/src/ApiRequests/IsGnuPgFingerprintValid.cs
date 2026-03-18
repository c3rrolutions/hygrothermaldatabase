using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Logging;
using Database.Services;
using GraphQL;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Database.ApiRequests;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Response contains errors.")
    ]
    internal static partial void ResponseErrors(
        this ILogger<IsGnuPgFingerprintValid> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

public sealed class IsGnuPgFingerprintValid(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<IsGnuPgFingerprintValid> logger
)
{
    private const string QueryFileName = "IsGnuPgFingerprintValid.graphql";

    public Uri GetGraphQlEndpoint =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record Institution(
        bool HasGnuPgKeyFingerprint
    );

    private sealed record Data(Institution? Institution);

    public async Task<bool> Do(
        string fingerprint,
        Guid institutionId,
        OffsetDateTime createdAt,
        CancellationToken cancellationToken
    )
    {
        var response = (await apiRequestService.QueryGraphQl<Data>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(QueryFileName),
                new
                {
                    fingerprint,
                    institutionId,
                    createdAt
                },
                "IsGnuPgFingerprintValid"
            ),
            cancellationToken
        ));
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response.Data.Institution?.HasGnuPgKeyFingerprint ?? false; ;
    }
}