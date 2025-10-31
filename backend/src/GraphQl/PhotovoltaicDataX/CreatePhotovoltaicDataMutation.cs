using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;
using HotChocolate;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Database.Extensions;
using Database.GraphQl.DataX;
using Database.ApiRequests;

namespace Database.GraphQl.PhotovoltaicDataX;

public sealed record CreatePhotovoltaicDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod
) : IValidateCreateInput
{
    public PhotovoltaicData ToDomainModel(Guid userId)
    {
        return new(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel()
        );
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreatePhotovoltaicDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_COMPONENT,
    ILLEGAL_CREATED_AT,
    UNKNOWN_CREATOR,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreatePhotovoltaicDataError(
    CreatePhotovoltaicDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreatePhotovoltaicDataErrorCode>(Code, Message, Path);

public sealed record CreatePhotovoltaicDataPayload(
    PhotovoltaicData? PhotovoltaicData,
    IReadOnlyCollection<CreatePhotovoltaicDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreatePhotovoltaicDataMutation
: CreateDataMutationBase<PhotovoltaicData, CreatePhotovoltaicDataPayload, CreatePhotovoltaicDataError, CreatePhotovoltaicDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreatePhotovoltaicDataPayload NewPayload(
        PhotovoltaicData? data,
        IReadOnlyCollection<CreatePhotovoltaicDataError>? errors
    ) => new(data, errors);

    protected override CreatePhotovoltaicDataError NewError(
        CreatePhotovoltaicDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreatePhotovoltaicDataPayload> CreatePhotovoltaicDataAsync(
        CreatePhotovoltaicDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IComponentByIdDataLoader componentByIdDataLoader,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreatePhotovoltaicDataErrorCode.UNAUTHENTICATED,
                CreatePhotovoltaicDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await ValidateAsync(
                input,
                CreatePhotovoltaicDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreatePhotovoltaicDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreatePhotovoltaicDataErrorCode.UNKNOWN_CREATOR,
                cancellationToken
            )
            ).Failed(out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var photovoltaicData = input.ToDomainModel(currentUser.Uuid);
        context.PhotovoltaicData.Add(photovoltaicData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                photovoltaicData,
                CreatePhotovoltaicDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(photovoltaicData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(photovoltaicData, null);
    }
}