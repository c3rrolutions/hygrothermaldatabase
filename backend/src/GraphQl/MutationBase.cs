using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using static Database.ApiRequests.QueryCurrentUser;

namespace Database.GraphQl;

public abstract class MutationBase<TData, TPayload, TError, TErrorCode>
where TData : class
where TPayload : class
{
    protected abstract TPayload NewPayload(TData? data, IReadOnlyCollection<TError>? errors);
    protected abstract TError NewError(TErrorCode errorCode, string message, IReadOnlyList<string> path);

    protected async Task<Result<CurrentUser, TPayload>> AuthorizeAsync(
        TErrorCode unauthenticatedErrorCode,
        TErrorCode unauthorizedErrorCode,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await authorization.GetCurrentUserAsync(cancellationToken);
        if (currentUser is null)
        {
            return new Result<CurrentUser, TPayload>.Error(
                NewPayload(null, [
                    NewError(
                        unauthenticatedErrorCode,
                        "The user is not authenticated.",
                        []
                    )
                ])
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new Result<CurrentUser, TPayload>.Error(
                NewPayload(null, [
                    NewError(
                        unauthorizedErrorCode,
                        "The current user is not authorized to perform this mutation in this database.",
                        []
                    )
                ])
            );
        }
        return new Result<CurrentUser, TPayload>.Data(currentUser);
    }
}