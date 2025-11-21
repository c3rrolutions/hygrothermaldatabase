using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Services;
using GraphQL;
using GreenDonut;

namespace Database.ApiRequests;

public static class DataDataLoader
{
    private const string QueryFileName = "Data.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        appSettings.MetabaseGraphQlEndpoint;

    public sealed record Data(
        Guid Uuid,
        DataKind Kind
    );

    public sealed record Database(
        Guid Uuid,
        Data? Data
    );

    private sealed record DataData(
        Database? Database
    );

    [DataLoader]
    public static async Task<Dictionary<(Guid DatabaseId, Guid DataId, DataKind DataKind), Database>> GetDataByDatabaseAndIdAndKindAsync(
        IReadOnlyList<(Guid DatabaseId, Guid DataId, DataKind DataKind)> databaseIdsAndDataIdsAndDataKinds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        var keysAndDatabases = await Task.WhenAll(
            databaseIdsAndDataIdsAndDataKinds
            .Select(async databaseIdAndDataIdAndDataKind =>
                (
                    Key: databaseIdAndDataIdAndDataKind,
                    Database: (await apiRequestService.QueryGraphQl<DataData>(
                        GetGraphQlEndpoint(appSettings),
                        new GraphQLRequest(
                            await GraphQlQueryHelpers.Construct(QueryFileName),
                            new
                            {
                                databaseIdAndDataIdAndDataKind.DatabaseId,
                                databaseIdAndDataIdAndDataKind.DataId,
                                databaseIdAndDataIdAndDataKind.DataKind
                            },
                            null
                        ),
                        cancellationToken
                    ))
                    .Data
                    ?.Database
                )
            )
        );
        return keysAndDatabases
            .Where(_ => _.Database is not null)
            .ToDictionary(
                _ => (
                    _.Key.DatabaseId,
                    _.Key.DataId,
                    _.Key.DataKind
                ),
                _ => _.Database!
        );
    }
}