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
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

public sealed record DeleteGetHttpsResourceInput(
    Guid GetHttpsResourceId
);

[SuppressMessage("Naming", "CA1707")]
public enum DeleteGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_RESOURCE,
    ROOT_RESOURCE,
    NOT_PENDING_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED,
    HAS_CHILDREN
}

public sealed record DeleteGetHttpsResourceError(
    DeleteGetHttpsResourceErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteGetHttpsResourceErrorCode>(Code, Message, Path);

public sealed record DeleteGetHttpsResourcePayload(
    GetHttpsResource? GetHttpsResource,
    IReadOnlyCollection<DeleteGetHttpsResourceError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteGetHttpsResourceMutation
: DataMutationBase<GetHttpsResource, DeleteGetHttpsResourcePayload, DeleteGetHttpsResourceError, DeleteGetHttpsResourceErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override DeleteGetHttpsResourcePayload NewPayload(
        GetHttpsResource? data,
        IReadOnlyCollection<DeleteGetHttpsResourceError>? errors
    ) => new(data, errors);

    protected override DeleteGetHttpsResourceError NewError(
        DeleteGetHttpsResourceErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<DeleteGetHttpsResourcePayload> DeleteGetHttpsResourceAsync(
        DeleteGetHttpsResourceInput input,
        ApplicationDbContext context,
        ResponseApprovalService responseApprovalService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteGetHttpsResourceErrorCode.UNAUTHENTICATED,
                DeleteGetHttpsResourceErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var resource = await context.GetHttpsResourcesWithData
            .Where(_ => _.Id == input.GetHttpsResourceId)
            .SingleOrDefaultAsync(cancellationToken);
        if (resource is null)
        {
            return NewPayload(
                null,
                [NewError(
                    DeleteGetHttpsResourceErrorCode.UNKNOWN_RESOURCE,
                    "There is no resource with the given ID.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )]
            );
        }

        var errors = new List<DeleteGetHttpsResourceError>();
        if (resource.IsRoot())
        {
            errors.Add(
                NewError(
                    DeleteGetHttpsResourceErrorCode.ROOT_RESOURCE,
                    $"This is the root resource of the data set with the ID {resource.DataId}. You cannot delete it.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )
            );
        }
        if (resource.Data is not null && resource.Data.PublishingState is not PublishingState.PENDING)
        {
            errors.Add(
                NewError(
                    DeleteGetHttpsResourceErrorCode.NOT_PENDING_DATA,
                    $"The data set of the resource is not pending but {resource.Data.PublishingState}. You cannot delete the data set's resources. However, you may retract the data set.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )
            );
        }
        if (await context.Entry(resource)
            .Collection(_ => _.Children)
            .Query()
            .AnyAsync(cancellationToken)
        )
        {
            errors.Add(
                NewError(
                    DeleteGetHttpsResourceErrorCode.HAS_CHILDREN,
                    $"The resource has children. Delete those first.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        context.Remove(resource);
        await context.SaveChangesAsync(cancellationToken);

        if (resource.Data is not null && (await CreateResponseApprovalAsync(
                resource.Data,
                DeleteGetHttpsResourceErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var deleteResponseApprovalErrorPayload)
        )
        {
            context.GetHttpsResources.Add(resource);
            await context.SaveChangesAsync(cancellationToken);
            return deleteResponseApprovalErrorPayload;
        }

        return NewPayload(resource, null);
    }
}
