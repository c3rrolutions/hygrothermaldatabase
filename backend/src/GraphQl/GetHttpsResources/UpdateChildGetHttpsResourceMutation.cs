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

public sealed record UpdateChildGetHttpsResourceInput(
    Guid GetHttpsResourceId,
    string Description,
    Guid DataFormatId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput AppliedConversionMethod
) : IValidateGetHttpsResourceInput;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateChildGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_RESOURCE,
    UNKNOWN_DATA_FORMAT,
    UNKNOWN_METHOD,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record UpdateChildGetHttpsResourceError(
    UpdateChildGetHttpsResourceErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateChildGetHttpsResourceErrorCode>(Code, Message, Path);

public sealed record UpdateChildGetHttpsResourcePayload(
    GetHttpsResource? GetHttpsResource,
    IReadOnlyCollection<UpdateChildGetHttpsResourceError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateChildGetHttpsResourceMutation
: DataMutationBase<GetHttpsResource, UpdateChildGetHttpsResourcePayload, UpdateChildGetHttpsResourceError, UpdateChildGetHttpsResourceErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override UpdateChildGetHttpsResourcePayload NewPayload(
        GetHttpsResource? data,
        IReadOnlyCollection<UpdateChildGetHttpsResourceError>? errors
    ) => new(data, errors);

    protected override UpdateChildGetHttpsResourceError NewError(
        UpdateChildGetHttpsResourceErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<UpdateChildGetHttpsResourcePayload> UpdateChildGetHttpsResourceAsync(
        UpdateChildGetHttpsResourceInput input,
        ApplicationDbContext context,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        IMethodByIdDataLoader methodByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateChildGetHttpsResourceErrorCode.UNAUTHENTICATED,
                UpdateChildGetHttpsResourceErrorCode.UNAUTHORIZED,
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
                    UpdateChildGetHttpsResourceErrorCode.UNKNOWN_RESOURCE,
                    "Unknown child GET HTTPS resource, the ID may be wrong or the resource may be a root resource.",
                    [nameof(input), nameof(input.GetHttpsResourceId).ToLowerFirst()]
                )]
            );
        }

        var errors = new List<UpdateChildGetHttpsResourceError>();
        var validateResourceResult = await ValidateGetHttpsResourceAsync(
            input,
            [nameof(input)],
            dataFormatByIdDataLoader,
            UpdateChildGetHttpsResourceErrorCode.UNKNOWN_DATA_FORMAT,
            cancellationToken
        );
        if (validateResourceResult.Failed(out var dataFormat, out var validateResourceErrors))
        {
            errors.AddRange(validateResourceErrors);
        }
        if (await methodByIdDataLoader.LoadAsync(input.AppliedConversionMethod.MethodId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    UpdateChildGetHttpsResourceErrorCode.UNKNOWN_METHOD,
                    "The applied conversion method does not exist",
                    [nameof(input), nameof(input.AppliedConversionMethod).ToLowerFirst(), nameof(input.AppliedConversionMethod.MethodId).ToLowerFirst()]
                )
            );
        }
        // Note that `dataFormat` is only `null`, when `validateResourceResult` failed.
        if (errors.Count >= 1 || dataFormat is null)
        {
            return NewPayload(null, errors);
        }

        resource.UpdateChild(
            input.Description,
            input.DataFormatId,
            dataFormat.Extension,
            input.ArchivedFilesMetaInformation.Select(_ => _.ToDomainModel()).ToList(),
            input.AppliedConversionMethod.ToDomainModel()
        );
        await context.SaveChangesAsync(cancellationToken);

        if (resource.Data is not null && (await CreateResponseApprovalAsync(
                resource.Data,
                UpdateChildGetHttpsResourceErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
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