using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;
using Database.Utilities;
using GraphQL.Client.Abstractions.Utilities;
using GreenDonut.Data;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.GetHttpsResources;

public sealed record CreateGetHttpsResourceInput(
    Guid DataId,
    DataKind DataKind,
    string Description,
    Guid DataFormatId,
    Guid ParentId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput AppliedConversionMethod
) : IIdentifyDataInput, IValidateGetHttpsResourceInput
{
    public GetHttpsResource ToDomainModel(string? fileExtension)
    {
        return new(
            Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            DataFormatId,
            fileExtension,
            DataKind is DataKind.CALORIMETRIC_DATA ? DataId : null,
            DataKind is DataKind.GEOMETRIC_DATA ? DataId : null,
            DataKind is DataKind.HYGROTHERMAL_DATA ? DataId : null,
            DataKind is DataKind.LIFE_CYCLE_DATA ? DataId : null,
            DataKind is DataKind.OPTICAL_DATA ? DataId : null,
            DataKind is DataKind.PHOTOVOLTAIC_DATA ? DataId : null,
            ParentId,
            ArchivedFilesMetaInformation.Select(i => i.ToDomainModel()).ToList(),
            AppliedConversionMethod.ToDomainModel()
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
    UNKNOWN_PARENT,
    ILLEGAL_PARENT,
    UNKNOWN_DATA_FORMAT,
    UNKNOWN_METHOD,
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

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<CreateGetHttpsResourcePayload> CreateGetHttpsResourceAsync(
        CreateGetHttpsResourceInput input,
        ApplicationDbContext context,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        IMethodByIdDataLoader methodByIdDataLoader,
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
            ).Failed(out var _, out var authorizeErrorPayload)
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
        var validateResourceResult = await ValidateGetHttpsResourceAsync(
            input,
            [nameof(input)],
            dataFormatByIdDataLoader,
            CreateGetHttpsResourceErrorCode.UNKNOWN_DATA_FORMAT,
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
                    CreateGetHttpsResourceErrorCode.UNKNOWN_METHOD,
                    "The applied conversion method does not exist",
                    [nameof(input), nameof(input.AppliedConversionMethod).ToLowerFirst(), nameof(input.AppliedConversionMethod.MethodId).ToLowerFirst()]
                )
            );
        }
        // Note that `dataFormat` is only `null`, when `validateResourceResult` failed.
        if (errors.Count > 0 || dataFormat is null)
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
