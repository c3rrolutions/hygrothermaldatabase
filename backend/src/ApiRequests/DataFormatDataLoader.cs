using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;

namespace Database.ApiRequests;

public static class DataFormatDataLoader
{
    private const string QueryFileName = "DataFormats.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record DataFormat(
        Guid Uuid,
        string Name,
        string? Extension,
        string MediaType,
        Uri? SchemaLocator
    ) : IIdNode;

    private sealed record DataFormatsData(
        Connection<DataFormat>? Connection
    ) : IConnectionData<DataFormat>;

    [DataLoader]
    public static Task<Dictionary<Guid, DataFormat>> GetDataFormatByIdAsync(
        IReadOnlyList<Guid> dataFormatIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<DataFormatsData, DataFormat>(
            dataFormatIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}