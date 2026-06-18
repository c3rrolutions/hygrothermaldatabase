using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record ResetDataAccessPolicyInput
(
    DataReferenceInput? Data
);

[SuppressMessage("Naming", "CA1707")]
public enum ResetDataAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ResetDataAccessPolicyError(
    ResetDataAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ResetDataAccessPolicyErrorCode>(Code, Message, Path);

public sealed record ResetDataAccessPolicyPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<ResetDataAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ResetDataAccessPolicyMutation
: DataMutationBase<DataAccessPolicy, ResetDataAccessPolicyPayload, ResetDataAccessPolicyError, ResetDataAccessPolicyErrorCode>
{
    protected override ResetDataAccessPolicyPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<ResetDataAccessPolicyError>? errors
    ) => new(data, errors);

    protected override ResetDataAccessPolicyError NewError(
        ResetDataAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<ResetDataAccessPolicyPayload> ResetDataAccessPolicyAsync(
        ResetDataAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ResetDataAccessPolicyErrorCode.UNAUTHENTICATED,
                ResetDataAccessPolicyErrorCode.UNAUTHORIZED,
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
                    ResetDataAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var data, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .Include(_ => _.UserAccessPolicies)
            .Include(_ => _.InstitutionAccessPolicies)
            .Include(_ => _.OpenIdConnectApplicationAccessPolicies)
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        dataAccessPolicy.Reset();
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(dataAccessPolicy, null);
    }
}
