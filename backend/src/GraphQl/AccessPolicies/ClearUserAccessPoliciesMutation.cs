using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record ClearUserAccessPoliciesInput
(
    DataReferenceInput? Data
);

[SuppressMessage("Naming", "CA1707")]
public enum ClearUserAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ClearUserAccessPoliciesError(
    ClearUserAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearUserAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearUserAccessPoliciesPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<ClearUserAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearUserAccessPoliciesMutation
: DataMutationBase<DataAccessPolicy, ClearUserAccessPoliciesPayload, ClearUserAccessPoliciesError, ClearUserAccessPoliciesErrorCode>
{
    protected override ClearUserAccessPoliciesPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<ClearUserAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearUserAccessPoliciesError NewError(
        ClearUserAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<ClearUserAccessPoliciesPayload> ClearUserAccessPoliciesAsync(
        ClearUserAccessPoliciesInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearUserAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearUserAccessPoliciesErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if (input.Data is not null)
        {
            if ((await FetchDataAsync(
                    input.Data,
                    ClearUserAccessPoliciesErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .Include(_ => _.UserAccessPolicies)
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        dataAccessPolicy.UserAccessPolicies.Clear();
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(dataAccessPolicy, null);
    }
}
