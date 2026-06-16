using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using HotChocolate;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class DataFormatDataLoader
{
    private const string QueryFileName = "DataFormats.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record DataFormat(
        [property: GraphQLIgnore] Guid Id,
        string Name,
        string? Extension,
        string MediaType,
        Uri? SchemaLocator
    ) : IIdNode<Guid>
    {
        public Guid Uuid => Id;
    }

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
        return QueryByIdDataLoader.GetByIdAsync<Guid, DataFormatsData, DataFormat>(
            dataFormatIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}