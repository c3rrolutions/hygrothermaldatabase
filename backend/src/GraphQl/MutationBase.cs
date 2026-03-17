using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using static Database.ApiRequests.QueryCurrentUserOrInstitution;

namespace Database.GraphQl;

public abstract class MutationBase<TData, TPayload, TError, TErrorCode>
where TData : class
where TPayload : class
{
    protected abstract TPayload NewPayload(TData? data, IReadOnlyCollection<TError>? errors);
    protected abstract TError NewError(TErrorCode errorCode, string message, IReadOnlyList<string> path);

    protected async Task<Result<CurrentUserOrInstitution, TPayload>> AuthorizeAsync(
        TErrorCode unauthenticatedErrorCode,
        TErrorCode unauthorizedErrorCode,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsAuthenticated(cancellationToken))
        {
            return new Result<CurrentUserOrInstitution, TPayload>.Error(
                NewPayload(null, [
                    NewError(
                        unauthenticatedErrorCode,
                        "You are not authenticated.",
                        []
                    )
                ])
            );
        }
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return new Result<CurrentUserOrInstitution, TPayload>.Error(
                NewPayload(null, [
                    NewError(
                        unauthorizedErrorCode,
                        "You are not authorized to perform this mutation in this database.",
                        []
                    )
                ])
            );
        }
        return new Result<CurrentUserOrInstitution, TPayload>.Data(
            await authorization.FetchCurrentUserOrInstitutionAsync(cancellationToken)
        );
    }
}