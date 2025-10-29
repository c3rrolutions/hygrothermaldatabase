using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Authorization;
using Database.Enumerations;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Database.Services;
using GraphQL.Client.Abstractions.Utilities;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Database.Utilities;
using HotChocolate.Data;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore.Query;
using Database.GraphQl.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Database.Extensions;

namespace Database.GraphQl.GetHttpsResources;

public sealed record CreateGetHttpsResourceInput(
    string Description,
    Guid DataFormatId,
    Guid DataId,
    DataKind DataKind,
    Guid? ParentId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput? AppliedConversionMethod
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum CreateGetHttpsResourceErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED
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

        var getHttpsResource = new GetHttpsResource(
            input.Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            input.DataFormatId,
            input.DataKind == DataKind.CALORIMETRIC_DATA ? input.DataId : null,
            input.DataKind == DataKind.GEOMETRIC_DATA ? input.DataId : null,
            input.DataKind == DataKind.HYGROTHERMAL_DATA ? input.DataId : null,
            input.DataKind == DataKind.OPTICAL_DATA ? input.DataId : null,
            input.DataKind == DataKind.PHOTOVOLTAIC_DATA ? input.DataId : null,
            input.ParentId,
            input.ArchivedFilesMetaInformation.Select(i => i.ToDomainModel()).ToList(),
            input?.AppliedConversionMethod.ToDomainModel()
        );
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