using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Services;
using static Database.ApiRequests.QueryCurrentUser;

namespace Database.GraphQl;

public abstract class MutationsBase
{
    protected static async Task<TPayload> AuthorizeAsync<TPayload, TError, TErrorCode>(
        TErrorCode unauthenticatedErrorCode,
        TErrorCode unauthorizedErrorCode,
        Func<IReadOnlyCollection<TError>?, TPayload> newPayload,
        Func<TErrorCode, string, IReadOnlyList<string>, TError> newError,
        UserService userService,
        Func<CurrentUser, Task<TPayload>> then,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return newPayload([
                newError(
                    unauthenticatedErrorCode,
                    "The user is not authenticated.",
                    []
                )
            ]);
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return newPayload([
                newError(
                    unauthorizedErrorCode,
                    "The current user is not authorized to perform this mutation in this database.",
                    []
                )
            ]);
        }
        return await then(currentUser);
    }
}