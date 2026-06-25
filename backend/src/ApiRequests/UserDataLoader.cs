using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Services;
using GreenDonut;
using HotChocolate;
using static Database.ApiRequests.QueryByIdDataLoader;

namespace Database.ApiRequests;

public static class UserDataLoader
{
    private const string QueryFileName = "Users.graphql";

    public static Uri GetGraphQlEndpoint(AppSettings appSettings) =>
        QueryByIdDataLoader.GetGraphQlEndpoint(appSettings);

    public sealed record User(
        [property: GraphQLIgnore] Guid Id,
        string Name
    ) : IIdNode<Guid>
    {
        public Guid Uuid => Id;
    }

    private sealed record UsersData(
        Connection<User>? Connection
    ) : IConnectionData<User>;

    [DataLoader]
    public static Task<Dictionary<Guid, User>> GetUserByIdAsync(
        IReadOnlyList<Guid> userIds,
        ApiRequestService apiRequestService,
        AppSettings appSettings,
        CancellationToken cancellationToken
    )
    {
        return QueryByIdDataLoader.GetByIdAsync<Guid, UsersData, User>(
            userIds,
            [QueryFileName],
            apiRequestService,
            appSettings,
            cancellationToken
        );
    }
}