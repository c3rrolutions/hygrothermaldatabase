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
        this ILogger<UpdateDatabase> logger,
        [TagProvider(typeof(GraphQlErrorsTagProvider), nameof(GraphQlErrorsTagProvider.RecordTags))] GraphQLError[] errors
    );
}

public sealed class UpdateDatabase(
    AppSettings appSettings,
    ApiRequestService apiRequestService,
    ILogger<UpdateDatabase> logger
)
{
    private const string UpdateDatabaseFileName = "UpdateDatabase.graphql";

    public Uri GetGraphQlEndpoint =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record UpdateDatabaseInput(
        Guid DatabaseId,
        string Name,
        string Description,
        Uri Locator
    );

    public sealed record UpdateDatabasePayload(
        QueryDatabase.Database? Database,
        IReadOnlyList<UpdateDatabaseError>? Errors
    );

    [SuppressMessage("Naming", "CA1707")]
    public enum UpdateDatabaseErrorCode
    {
        UNKNOWN,
        UNAUTHORIZED,
        UNKNOWN_DATABASE
    }

    public sealed record UpdateDatabaseError(
        UpdateDatabaseErrorCode Code,
        string Message,
        IReadOnlyList<string> Path
    );

    private sealed record UpdateDatabaseData(UpdateDatabasePayload? UpdateDatabase);

    public async Task<UpdateDatabasePayload?> Do(
        UpdateDatabaseInput updateDatabaseInput,
        CancellationToken cancellationToken
        )
    {
        var response = (await apiRequestService.QueryGraphQl<UpdateDatabaseData>(
            GetGraphQlEndpoint,
            new GraphQLRequest(
                await GraphQlQueryHelpers.Construct(
                    UpdateDatabaseFileName
                ),
                new
                {
                    input = updateDatabaseInput
                },
                "UpdateDatabase"
            ),
            cancellationToken
        ));
        if (response.Errors is not null)
        {
            logger.ResponseErrors(response.Errors);
        }
        return response.Data.UpdateDatabase;
    }
}