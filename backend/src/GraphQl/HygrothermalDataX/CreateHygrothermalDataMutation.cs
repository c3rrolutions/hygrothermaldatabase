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
using Database.GraphQl.DataX;
using Database.Extensions;
using Database.ApiRequests;

namespace Database.GraphQl.HygrothermalDataX;

public sealed record CreateHygrothermalDataInput(
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
    public HygrothermalData ToDomainModel(Guid userId)
    {
        var hygrothermalData = new HygrothermalData(
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
        return hygrothermalData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateHygrothermalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_COMPONENT,
    ILLEGAL_CREATED_AT,
    UNKNOWN_CREATOR,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateHygrothermalDataError(
    CreateHygrothermalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateHygrothermalDataErrorCode>(Code, Message, Path);

public sealed record CreateHygrothermalDataPayload(
    HygrothermalData? HygrothermalData,
    IReadOnlyCollection<CreateHygrothermalDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateHygrothermalDataMutation
: CreateDataMutationBase<HygrothermalData, CreateHygrothermalDataPayload, CreateHygrothermalDataError, CreateHygrothermalDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateHygrothermalDataPayload NewPayload(
        HygrothermalData? data,
        IReadOnlyCollection<CreateHygrothermalDataError>? errors
    ) => new(data, errors);

    protected override CreateHygrothermalDataError NewError(
        CreateHygrothermalDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreateHygrothermalDataPayload> CreateHygrothermalDataAsync(
        CreateHygrothermalDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IComponentByIdDataLoader componentByIdDataLoader,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateHygrothermalDataErrorCode.UNAUTHENTICATED,
                CreateHygrothermalDataErrorCode.UNAUTHORIZED,
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
                CreateHygrothermalDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_CREATOR,
                cancellationToken
            )
            ).Failed(out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var hygrothermalData = input.ToDomainModel(currentUser.Uuid);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                hygrothermalData,
                CreateHygrothermalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(hygrothermalData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(hygrothermalData, null);
    }
}