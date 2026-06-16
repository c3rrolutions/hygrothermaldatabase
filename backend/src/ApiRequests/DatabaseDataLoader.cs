using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using HotChocolate;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public sealed class DatabaseDataLoader
{
    private const string QueryFileName = "Databases.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record Database(
        [property: GraphQLIgnore] Guid Id,
        string Name,
        string Description,
        Uri Locator,
        DatabaseVerificationState VerificationState,
        string VerificationCode,
        DatabaseOperatorEdge Operator,
        bool IsAuthorizedToUpdateNode,
        bool IsAuthorizedToVerifyNode
    ) : IIdNode<Guid>
    {
        public Guid Uuid => Id;
    }

    public enum DatabaseVerificationState
    {
        PENDING,
        VERIFIED
    }

    public sealed record DatabaseOperatorEdge(
        InstitutionDataLoader.Institution Node
    );

    private sealed record DatabasesData(
        Connection<Database>? Connection
    ) : IConnectionData<Database>;

    [DataLoader]
    public static Task<Dictionary<Guid, Database>> GetDatabaseByIdAsync(
        IReadOnlyList<Guid> componentIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<Guid, DatabasesData, Database>(
            componentIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}