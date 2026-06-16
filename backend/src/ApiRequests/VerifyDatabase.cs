using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Logging;
using Database.Services;
using GraphQL;
using Microsoft.Extensions.Logging;

namespace Database.ApiRequests;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Response contains errors.")
    ]
    internal static partial void ResponseErrors(
        this ILogger<VerifyDatabase> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

public sealed class VerifyDatabase(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<VerifyDatabase> logger
)
{
    private const string VerifyDatabaseFileName = "VerifyDatabase.graphql";

    public Uri GetGraphQlEndpoint =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record VerifyDatabaseInput(
        Guid DatabaseId
    );

    public sealed record VerifyDatabasePayload(
        DatabaseDataLoader.Database? Database,
        IReadOnlyList<VerifyDatabaseError>? Errors
    );

    [SuppressMessage("Naming", "CA1707")]
    public enum VerifyDatabaseErrorCode
    {
        UNKNOWN,
        UNAUTHORIZED,
        UNKNOWN_DATABASE
    }

    public sealed record VerifyDatabaseError(
        VerifyDatabaseErrorCode Code,
        string Message,
        IReadOnlyList<string> Path
    );

    private sealed record VerifyDatabaseData(VerifyDatabasePayload? VerifyDatabase);

    public async Task<VerifyDatabasePayload?> Do(
        VerifyDatabaseInput verifyDatabaseInput,
        CancellationToken cancellationToken
        )
    {
        var response = (await apiRequestService.QueryGraphQl<VerifyDatabaseData>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(
                    VerifyDatabaseFileName
                ),
                new
                {
                    input = verifyDatabaseInput
                },
                "VerifyDatabase"
            ),
            cancellationToken
        ));
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response.Data.VerifyDatabase;
    }
}