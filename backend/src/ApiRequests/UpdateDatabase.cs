using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GraphQL;

namespace Database.ApiRequests;

public sealed class UpdateDatabase
{
    private const string UpdateDatabaseFileName = "UpdateDatabase.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
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

    public static async Task<UpdateDatabasePayload?> Do(
        UpdateDatabaseInput updateDatabaseInput,
        AppSettings appSettings,
        ApiRequestService apiRequestService,
        CancellationToken cancellationToken
        )
    {
        return (await apiRequestService.QueryGraphQl<UpdateDatabaseData>(
            GetGraphQlEndpoint(appSettings),
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
        ))
        .Data
        .UpdateDatabase;
    }
}