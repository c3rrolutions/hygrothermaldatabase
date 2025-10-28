using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using GreenDonut.Data;
using HotChocolate.Types;
using System.Diagnostics.CodeAnalysis;
using Database.Enumerations;

namespace Database.GraphQl.DataApprovals;

public sealed record RemoveDataApprovalInput
(
    Guid DataId,
    DataKind DataKind,
    string Signature
);

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

public sealed class RemoveDataApprovalPayload
    : DataApprovalPayload<RemoveDataApprovalError>
{
    public RemoveDataApprovalPayload(
        DataApproval dataApproval
    )
        : base(dataApproval)
    {
    }

    public RemoveDataApprovalPayload(
        RemoveDataApprovalError error
    )
        : base(error)
    {
    }

    public RemoveDataApprovalPayload(
        DataApproval dataApproval,
        RemoveDataApprovalError error
    )
        : base(dataApproval, error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class RemoveDataApprovalMutation
{
    public async Task<RemoveDataApprovalPayload> RemoveDataApprovalAsync(
        RemoveDataApprovalInput input,
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add data approvals to this database.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, input.DataKind, cancellationToken);
        if (data is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            );
        }

        var approval = data.Approvals
            .Where(a => a.Signature.Trim() == input.Signature.Trim())
            .SingleOrDefault();
        if (approval is null)
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNKNOWN_APPROVAL,
                    $"Unknown data approval.",
                    [nameof(input), nameof(input.Signature).ToLowerFirst()]
                )
            );
        }

        data.Approvals.Remove(approval);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            data.Approvals.Add(approval);
            await context.SaveChangesAsync(cancellationToken);

            return new RemoveDataApprovalPayload(
                approval,
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new RemoveDataApprovalPayload(approval);
    }
}