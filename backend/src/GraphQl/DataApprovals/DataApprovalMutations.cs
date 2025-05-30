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

        if (!CommonAuthorization.IsAuthorizedToAddDataApprovalForInstitution(
            currentUser,
            input.CreatorId,
            cancellationToken
            )
        )
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to approval for the institution.",
                    []
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

        var data = await dataService.GetDataAsync(input.DataId, context, cancellationToken).ConfigureAwait(false);
        if (data is null)
        {
            return new AddDataApprovalPayload(
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        var approval = new DataApproval(
            DateTime.Now,
            input.Signature,
            input.KeyFingerprint,
            input.Query,
            input.Response,
            currentUser.Id,
            ReferenceType.FromInput(input.Statement)
        );

        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            data.Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Remove(data);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new AddDataApprovalPayload(
                approval,
                new AddDataApprovalError(
                    AddDataApprovalErrorCode.SIGNING_FAILED,
                    $"Signing failed with message: {ex.Message}",
                    []
                )
            );
        }
        return new AddDataApprovalPayload(approval);
    }
}