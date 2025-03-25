using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;

namespace Database.GraphQl.Approvals;

[ExtendObjectType(nameof(Mutation))]
public sealed class ApprovalMutations
{
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<AddApprovalPayload> AddApprovalToDataAsync(
        ApprovalInput input,
        ApplicationDbContext context,
        IUserService userService,
        IDataService dataService,
        IResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsAuthorizedToAddApprovalForInstitution(
            currentUser,
            input.CreatorId,
            cancellationToken
            )
        )
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to approval for the institution.",
                    []
                )
            );

        var data = await dataService.GetDataAsync(input.DataId, context, cancellationToken).ConfigureAwait(false);
        if (data == null)
        {
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        var approval = new DataApproval(
                DateTime.Now,
                input.Approval.Signature,
                input.Approval.KeyFingerprint,
                input.Approval.Query,
                input.Approval.Response,
                currentUser.Id);
        if (input.Approval.Publication != null)
        {
            approval.Publication = new Publication(
                input.Approval.Publication.Title,
                input.Approval.Publication.Abstract,
                input.Approval.Publication.Section,
                input.Approval.Publication.Authors,
                input.Approval.Publication.Doi,
                input.Approval.Publication.ArXiv,
                input.Approval.Publication.Urn,
                input.Approval.Publication.WebAddress);
        }
        if (input.Approval.Standard != null)
        {
            approval.Standard = new Standard(
                input.Approval.Standard.Title,
                input.Approval.Standard.Abstract,
                input.Approval.Standard.Section,
                input.Approval.Standard.Year,
                input.Approval.Standard.Standardizers,
                input.Approval.Standard.Locator);
        }
        data.Approvals.Add(approval);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            ((Data.DataX)data).Approval = await responseApprovalService.CreateResponseApproval(data, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Remove(data);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new AddApprovalPayload(
                approval,
                new AddApprovalError(
                    AddApprovalErrorCode.SIGNING_FAILED,
                    $"Signing failed with message: {ex.Message}",
                    []
                )
            );
        }
        return new AddApprovalPayload(approval);
    }
}