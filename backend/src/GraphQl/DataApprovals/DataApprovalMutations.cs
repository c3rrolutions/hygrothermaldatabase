using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.References;
using Database.Services;
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
        DataService dataService,
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

        var data = await dataService.GetDataAsync(input.DataId, context, cancellationToken);
        if (data is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    [nameof(input), nameof(input.DataId).FirstCharToLower()]
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
                    [nameof(input), nameof(input.Statement).FirstCharToLower()]
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
                    [nameof(input), nameof(input.Statement).FirstCharToLower()]
                )
            );
        }

        var approval = new DataApproval(
            input.Timestamp,
            input.Signature,
            input.KeyFingerprint,
            input.Query,
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
            context.Remove(data);
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
}