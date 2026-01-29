using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Extensions;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using GreenDonut.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

public sealed record SetGetHttpsResourceParentInput(
    Guid GetHttpsResourceId,
    Guid ParentId,
    ToTreeVertexAppliedConversionMethodInput AppliedConversionMethod
);

[SuppressMessage("Naming", "CA1707")]
public enum SetGetHttpsResourceParentErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_RESOURCE,
    UNKNOWN_PARENT,
    ILLEGAL_PARENT,
    UNKNOWN_METHOD,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record SetGetHttpsResourceParentError(
    SetGetHttpsResourceParentErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetGetHttpsResourceParentErrorCode>(Code, Message, Path);

public sealed record SetGetHttpsResourceParentPayload(
    GetHttpsResource? GetHttpsResource,
    IReadOnlyCollection<SetGetHttpsResourceParentError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetGetHttpsResourceParentMutation
: DataMutationBase<GetHttpsResource, SetGetHttpsResourceParentPayload, SetGetHttpsResourceParentError, SetGetHttpsResourceParentErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override SetGetHttpsResourceParentPayload NewPayload(
        GetHttpsResource? data,
        IReadOnlyCollection<SetGetHttpsResourceParentError>? errors
    ) => new(data, errors);

    protected override SetGetHttpsResourceParentError NewError(
        SetGetHttpsResourceParentErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetGetHttpsResourceParentPayload> SetGetHttpsResourceParentAsync(
        SetGetHttpsResourceParentInput input,
        ApplicationDbContext context,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        IMethodByIdDataLoader methodByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetGetHttpsResourceParentErrorCode.UNAUTHENTICATED,
                SetGetHttpsResourceParentErrorCode.UNAUTHORIZED,
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
            .Where(_ => _.ParentId != null)
            .SingleOrDefaultAsync(cancellationToken);
        if (resource is null)
        {
            return NewPayload(null,
                [NewError(
                    SetGetHttpsResourceParentErrorCode.UNKNOWN_RESOURCE,
                    "Unknown child GET HTTPS resource, the ID may be wrong or the resource may be a root resource.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )]
            );
        }

        var errors = new List<SetGetHttpsResourceParentError>();
        var parent =
            await context.GetHttpsResources
            .SingleOrDefaultAsync(_ =>
                _.Id == input.ParentId,
                cancellationToken
            );
        if (parent is null)
        {
            errors.Add(
                NewError(
                    SetGetHttpsResourceParentErrorCode.UNKNOWN_PARENT,
                    "There is not GET HTTPS resource with the parent ID.",
                    [nameof(input), nameof(input.ParentId).ToLowerFirst()]
                )
            );
        }
        if (parent is not null && parent.DataId != resource.DataId)
        {
            errors.Add(
                NewError(
                    SetGetHttpsResourceParentErrorCode.ILLEGAL_PARENT,
                    $"The parent belongs to the data set with ID {parent.DataId} which is not the data set of the resource, namely the one with the ID {resource.DataId}.",
                    [nameof(input), nameof(input.ParentId).ToLowerFirst()]
                )
            );
        }
        if (await methodByIdDataLoader.LoadAsync(input.AppliedConversionMethod.MethodId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetGetHttpsResourceParentErrorCode.UNKNOWN_METHOD,
                    "The applied conversion method does not exist",
                    [nameof(input), nameof(input.AppliedConversionMethod).ToLowerFirst(), nameof(input.AppliedConversionMethod.MethodId).ToLowerFirst()]
                )
            );
        }
        // Note that `dataFormat` is only `null`, when `validateResourceResult` failed.
        if (errors.Count >= 1)
        {
            return NewPayload(null, errors);
        }

        resource.SetParent(
            input.ParentId,
            input.AppliedConversionMethod.ToDomainModel()
        );
        await context.SaveChangesAsync(cancellationToken);

        if (resource.Data is not null && (await CreateResponseApprovalAsync(
                resource.Data,
                SetGetHttpsResourceParentErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            // TODO rollback?
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(resource, null);
    }
}