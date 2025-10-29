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
using Database.Extensions;

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
    RootGetHttpsResourceInput RootResource,
    double[] GValues,
    double[] UValues
)
{
    public CalorimetricData ToDomainModel(Guid userId)
    {
        var calorimetricData = new CalorimetricData(
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
        calorimetricData.Resources.Add(RootResource.ToDomainModel());
        return calorimetricData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateCalorimetricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
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
: DataMutationBase<CalorimetricData, CreateCalorimetricDataPayload, CreateCalorimetricDataError, CreateCalorimetricDataErrorCode>
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

        var calorimetricData = input.ToDomainModel(currentUser.Uuid);
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