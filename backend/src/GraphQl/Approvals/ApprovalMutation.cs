using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.Approvals;

[ExtendObjectType(nameof(Mutation))]
public sealed class ApprovalMutation
{
    [Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<AddApprovalPayload> AddApprovalToDataAsync(
        ApprovalInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        IDataService dataService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNKNOWN,
                    $"The current user is not known.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsAuthorizedToAddApprovalForInstitution(
            currentUser,
            input.CreatorId,
            appSettings,
            httpClientFactory,
            httpContextAccessor,
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

        var data = await dataService.GetDataAsync(input.DataId, cancellationToken).ConfigureAwait(false);
        if (data == null)
        {
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNKNOWN,
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
        data.Approvals.Add(approval);
        //data.Approval = new ResponseApproval(
        //    DateTime.Now,
        //    input.ResponseApproval.Signature,
        //    input.ResponseApproval.KeyFingerprint,
        //    input.ResponseApproval.Query,
        //    input.ResponseApproval.Response);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddApprovalPayload(approval);
    }
}