using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Authorization;
using Database.Enumerations;
using HotChocolate.Types;
using Database.Services;
using Microsoft.Extensions.Logging;
using Database.Utilities;
using GreenDonut.Data;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using Microsoft.EntityFrameworkCore;
using Database.ApiRequests;

namespace Database.GraphQl.GetHttpsResources;

public sealed record CreateGetHttpsResourceInput(
    string Description,
    Guid DataFormatId,
    Guid DataId,
    DataKind DataKind,
    Guid? ParentId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput? AppliedConversionMethod
) : IIdentifyDataInput
{
    public GetHttpsResource ToDomainModel(string? fileExtension)
    {
        return new GetHttpsResource(
            Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            DataFormatId,
            fileExtension,
            DataKind == DataKind.CALORIMETRIC_DATA ? DataId : null,
            DataKind == DataKind.GEOMETRIC_DATA ? DataId : null,
            DataKind == DataKind.HYGROTHERMAL_DATA ? DataId : null,
            DataKind == DataKind.OPTICAL_DATA ? DataId : null,
            DataKind == DataKind.PHOTOVOLTAIC_DATA ? DataId : null,
            ParentId,
            ArchivedFilesMetaInformation.Select(i => i.ToDomainModel()).ToList(),
            AppliedConversionMethod?.ToDomainModel()
        );
    }
}

[SuppressMessage("Naming", "CA1707")]
public enum CreateGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATA,
    APPLIED_CONVERSION_METHOD_WITHOUT_PARENT,
    CREATING_RESPONSE_APPROVAL_FAILED,
    UNKNOWN_PARENT,
    ILLEGAL_PARENT,
    UNKNOWN_DATA_FORMAT
}

public sealed record CreateGetHttpsResourceError(
    CreateGetHttpsResourceErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateGetHttpsResourceErrorCode>(Code, Message, Path);

public sealed record CreateGetHttpsResourcePayload(
    GetHttpsResource? GetHttpsResource,
    IReadOnlyCollection<CreateGetHttpsResourceError>? Errors
) : Payload;

public static partial class CreateGetHttpsResourceMutationLogging
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Recomputing the hash value of the GET HTTPS resource with ID {Id} failed."
    )]
    public static partial void FailedRecomputingHashValue(this ILogger<CreateGetHttpsResourceMutation> logger, Guid id, Exception exception);
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateGetHttpsResourceMutation
: DataMutationBase<GetHttpsResource, CreateGetHttpsResourcePayload, CreateGetHttpsResourceError, CreateGetHttpsResourceErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateGetHttpsResourcePayload NewPayload(
        GetHttpsResource? data,
        IReadOnlyCollection<CreateGetHttpsResourceError>? errors
    ) => new(data, errors);

    protected override CreateGetHttpsResourceError NewError(
        CreateGetHttpsResourceErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreateGetHttpsResourcePayload> CreateGetHttpsResourceAsync(
        CreateGetHttpsResourceInput input,
        ApplicationDbContext context,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateGetHttpsResourceErrorCode.UNAUTHENTICATED,
                CreateGetHttpsResourceErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                CreateGetHttpsResourceErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var errors = new List<CreateGetHttpsResourceError>();
        var dataFormat = await dataFormatByIdDataLoader.LoadAsync(input.DataFormatId, cancellationToken);
        if (dataFormat is null)
        {
            errors.Add(
                NewError(
                    CreateGetHttpsResourceErrorCode.UNKNOWN_DATA_FORMAT,
                    "There is no data format with the format ID.",
                    [nameof(input), nameof(input.DataFormatId).ToLowerFirst()]
                )
            );
        }
        var parent = await context.GetHttpsResources
            .SingleOrDefaultAsync(_ =>
                _.Id == input.ParentId,
                cancellationToken
            );
        if (parent is null)
        {
            errors.Add(
                NewError(
                    CreateGetHttpsResourceErrorCode.UNKNOWN_PARENT,
                    "There is not GET HTTPS resource with the parent ID.",
                    [nameof(input), nameof(input.ParentId).ToLowerFirst()]
                )
            );
        }
        if (parent is not null && parent.DataId != input.DataId)
        {
            errors.Add(
                NewError(
                    CreateGetHttpsResourceErrorCode.ILLEGAL_PARENT,
                    $"The parent belongs to the data set with ID {parent.DataId} which is not the data set of the resource to be created, namely the one with the ID {input.DataId}.",
                    [nameof(input), nameof(input.ParentId).ToLowerFirst()]
                )
            );
        }
        if (input.AppliedConversionMethod is not null && input.ParentId is null)
        {
            errors.Add(
                NewError(
                    CreateGetHttpsResourceErrorCode.APPLIED_CONVERSION_METHOD_WITHOUT_PARENT,
                    "The resource does not have a parent yet an applied conversion method was given. This method is supposed to have been applied to this resource's parent.",
                    [nameof(input), nameof(input.AppliedConversionMethod).ToLowerFirst()]
                )
            );
        }
        if (errors.Count >= 1)
        {
            return NewPayload(null, errors);
        }

        var getHttpsResource = input.ToDomainModel(dataFormat?.Extension);
        context.GetHttpsResources.Add(getHttpsResource);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                CreateGetHttpsResourceErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(getHttpsResource);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(getHttpsResource, null);
    }
}