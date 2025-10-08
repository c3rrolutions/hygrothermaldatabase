using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.References;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;

namespace Database.GraphQl.DataApprovals;

[ExtendObjectType(nameof(Mutation))]
public sealed class DataApprovalMutations
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<AddDataApprovalPayload> AddDataApprovalAsync(
        DataApprovalInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add data approvals to this database.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, cancellationToken);
        if (data is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    [nameof(input), nameof(input.DataId).ToLowerFirst()]
                )
            );
        }

        if (input.Statement.Standard is null
            && input.Statement.Publication is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.MISSING_STATEMENT,
                    "Both standard and publication are null.",
                    [nameof(input), nameof(input.Statement).ToLowerFirst()]
                )
            );
        }

        if (input.Statement.Standard is not null
            && input.Statement.Publication is not null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.AMBIGUOUS_STATEMENT,
                    "Both standard and publication are non-null.",
                    [nameof(input), nameof(input.Statement).ToLowerFirst()]
                )
            );
        }

        var approval = new DataApproval(
            input.Timestamp,
            input.Signature.Trim(),
            input.KeyFingerprint,
            input.Query.Trim(),
            input.Variables,
            input.Message,
            input.ApproverId,
            ReferenceType.FromInput(input.Statement)
        );

        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            data.Approvals.Remove(approval);
            await context.SaveChangesAsync(cancellationToken);

            return new AddDataApprovalPayload(
                approval,
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new AddDataApprovalPayload(approval);
    }

    public async Task<RemoveDataApprovalPayload> RemoveDataApprovalAsync(
        DataApprovalInput input,
        ApplicationDbContext context,
        UserService userService,
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
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new RemoveDataApprovalPayload(
                new RemoveDataApprovalError(
                    RemoveDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add data approvals to this database.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, cancellationToken);
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

        var approval = data.Approvals.Where(a => a.Signature.Trim() == input.Signature.Trim()).FirstOrDefault();
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