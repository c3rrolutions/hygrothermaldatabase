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
        var currentUser = await userService.GetCurrentUser(cancellationToken).ConfigureAwait(false);
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

        var data = await dataService.GetDataAsync(input.DataId, context, cancellationToken).ConfigureAwait(false);
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

        if (!CommonAuthorization.IsAuthorizedToAddDataApprovalForInstitution(
            currentUser,
            input.CreatorId
            )
        )
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add approvals for the institution.",
                    [nameof(input), nameof(input.CreatorId).FirstCharToLower()]
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
            input.Response,
            input.ApproverId,
            ReferenceType.FromInput(input.Statement)
        );

        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            context.Remove(data);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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