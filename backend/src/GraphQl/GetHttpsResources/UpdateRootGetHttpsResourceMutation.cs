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

public sealed record UpdateRootGetHttpsResourceInput(
    Guid GetHttpsResourceId,
    string Description,
    Guid DataFormatId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation
) : IValidateGetHttpsResourceInput;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateRootGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_RESOURCE,
    UNKNOWN_DATA_FORMAT,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record UpdateRootGetHttpsResourceError(
    UpdateRootGetHttpsResourceErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateRootGetHttpsResourceErrorCode>(Code, Message, Path);

public sealed record UpdateRootGetHttpsResourcePayload(
    GetHttpsResource? GetHttpsResource,
    IReadOnlyCollection<UpdateRootGetHttpsResourceError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateRootGetHttpsResourceMutation
: DataMutationBase<GetHttpsResource, UpdateRootGetHttpsResourcePayload, UpdateRootGetHttpsResourceError, UpdateRootGetHttpsResourceErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override UpdateRootGetHttpsResourcePayload NewPayload(
        GetHttpsResource? data,
        IReadOnlyCollection<UpdateRootGetHttpsResourceError>? errors
    ) => new(data, errors);

    protected override UpdateRootGetHttpsResourceError NewError(
        UpdateRootGetHttpsResourceErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<UpdateRootGetHttpsResourcePayload> UpdateRootGetHttpsResourceAsync(
        UpdateRootGetHttpsResourceInput input,
        ApplicationDbContext context,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        IMethodByIdDataLoader methodByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateRootGetHttpsResourceErrorCode.UNAUTHENTICATED,
                UpdateRootGetHttpsResourceErrorCode.UNAUTHORIZED,
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
            .Where(_ => _.ParentId == null)
            .SingleOrDefaultAsync(cancellationToken);
        if (resource is null)
        {
            return NewPayload(null,
                [NewError(
                    UpdateRootGetHttpsResourceErrorCode.UNKNOWN_RESOURCE,
                    "Unknown root GET HTTPS resource, the ID may be wrong or the resource may be a child of another resource.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )]
            );
        }

        var errors = new List<UpdateRootGetHttpsResourceError>();
        var validateResourceResult = await ValidateGetHttpsResourceAsync(
            input,
            [nameof(input)],
            dataFormatByIdDataLoader,
            UpdateRootGetHttpsResourceErrorCode.UNKNOWN_DATA_FORMAT,
            cancellationToken
        );
        if (validateResourceResult.Failed(out var dataFormat, out var validateResourceErrors))
        {
            errors.AddRange(validateResourceErrors);
        }
        // Note that `dataFormat` is only `null`, when `validateResourceResult` failed.
        if (errors.Count >= 1 || dataFormat is null)
        {
            return NewPayload(null, errors);
        }

        resource.UpdateRoot(
            input.Description,
            input.DataFormatId,
            dataFormat.Extension,
            input.ArchivedFilesMetaInformation.Select(_ => _.ToDomainModel()).ToArray()
        );
        await context.SaveChangesAsync(cancellationToken);

        if (resource.Data is not null && (await CreateResponseApprovalAsync(
                resource.Data,
                UpdateRootGetHttpsResourceErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
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