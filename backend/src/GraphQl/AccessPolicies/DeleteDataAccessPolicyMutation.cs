using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extensions;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed record DeleteDataAccessPolicyInput
(
    Guid DataId,
    DataKind DataKind
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum DeleteDataAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record DeleteDataAccessPolicyError(
    DeleteDataAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteDataAccessPolicyErrorCode>(Code, Message, Path);

public sealed record DeleteDataAccessPolicyPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<DeleteDataAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteDataAccessPolicyMutation
: DataMutationBase<DataAccessPolicy, DeleteDataAccessPolicyPayload, DeleteDataAccessPolicyError, DeleteDataAccessPolicyErrorCode>
{
    protected override DeleteDataAccessPolicyPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<DeleteDataAccessPolicyError>? errors
    ) => new(data, errors);

    protected override DeleteDataAccessPolicyError NewError(
        DeleteDataAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteDataAccessPolicyPayload> DeleteDataAccessPolicyAsync(
        DeleteDataAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteDataAccessPolicyErrorCode.UNAUTHENTICATED,
                DeleteDataAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                DeleteDataAccessPolicyErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        data.AccessPolicy = null;
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.AccessPolicy, null);
    }
}