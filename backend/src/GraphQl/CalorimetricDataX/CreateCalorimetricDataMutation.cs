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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Database.Extensions;
using Database.GraphQl.DataX;
using Database.ApiRequests;

namespace Database.GraphQl.CalorimetricDataX;

public sealed record CreateCalorimetricDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    double[] GValues,
    double[] UValues
) : IValidateCreateInput
{
    public CalorimetricData ToDomainModel(Guid userId)
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
            AppliedMethod.ToDomainModel(),
            GValues,
            UValues
        );
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateCalorimetricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_COMPONENT,
    ILLEGAL_CREATED_AT,
    UNKNOWN_CREATOR,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateCalorimetricDataError(
    CreateCalorimetricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateCalorimetricDataErrorCode>(Code, Message, Path);

public sealed record CreateCalorimetricDataPayload(
    CalorimetricData? CalorimetricData,
    IReadOnlyCollection<CreateCalorimetricDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateCalorimetricDataMutation
: CreateDataMutationBase<CalorimetricData, CreateCalorimetricDataPayload, CreateCalorimetricDataError, CreateCalorimetricDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateCalorimetricDataPayload NewPayload(
        CalorimetricData? data,
        IReadOnlyCollection<CreateCalorimetricDataError>? errors
    ) => new(data, errors);

    protected override CreateCalorimetricDataError NewError(
        CreateCalorimetricDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreateCalorimetricDataPayload> CreateCalorimetricDataAsync(
        CreateCalorimetricDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IComponentByIdDataLoader componentByIdDataLoader,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateCalorimetricDataErrorCode.UNAUTHENTICATED,
                CreateCalorimetricDataErrorCode.UNAUTHORIZED,
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
                CreateCalorimetricDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_CREATOR,
                cancellationToken
            )
            ).Failed(out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var calorimetricData = input.ToDomainModel(currentUser.Uuid);
        context.CalorimetricData.Add(calorimetricData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                calorimetricData,
                CreateCalorimetricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(calorimetricData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(calorimetricData, null);
    }
}