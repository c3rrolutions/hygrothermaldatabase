using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using GreenDonut.Data;
using HotChocolate.Types;

namespace Database.GraphQl.DataApprovals;

public sealed record RemoveDataApprovalInput
(
    Guid DataId,
    DataKind DataKind,
    string Signature
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum RemoveDataApprovalErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_APPROVAL,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record RemoveDataApprovalError(
    RemoveDataApprovalErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<RemoveDataApprovalErrorCode>(Code, Message, Path);

public sealed record RemoveDataApprovalPayload(
    DataApproval? DataApproval,
    IReadOnlyCollection<RemoveDataApprovalError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class RemoveDataApprovalMutation
: DataMutationBase<DataApproval, RemoveDataApprovalPayload, RemoveDataApprovalError, RemoveDataApprovalErrorCode>
{
    protected override RemoveDataApprovalPayload NewPayload(
        DataApproval? data,
        IReadOnlyCollection<RemoveDataApprovalError>? errors
    ) => new(data, errors);

    protected override RemoveDataApprovalError NewError(
        RemoveDataApprovalErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<RemoveDataApprovalPayload> RemoveDataApprovalAsync(
        RemoveDataApprovalInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                RemoveDataApprovalErrorCode.UNAUTHENTICATED,
                RemoveDataApprovalErrorCode.UNAUTHORIZED,
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
                RemoveDataApprovalErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var approval = data.Approvals
            .Where(a => a.Signature.Trim() == input.Signature.Trim())
            .SingleOrDefault();
        if (approval is null)
        {
            return NewPayload(
                null,
                [NewError(
                    RemoveDataApprovalErrorCode.UNKNOWN_APPROVAL,
                    $"Unknown data approval.",
                    [nameof(input), nameof(input.Signature).ToLowerFirst()]
                )]
            );
        }

        data.Approvals.Remove(approval);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                RemoveDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            data.Approvals.Add(approval);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(approval, null);
    }
}