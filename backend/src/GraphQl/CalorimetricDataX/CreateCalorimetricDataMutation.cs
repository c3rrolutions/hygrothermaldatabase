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

public sealed class CreateCalorimetricDataPayload
    : CalorimetricDataPayload<CreateCalorimetricDataError>
{
    public CreateCalorimetricDataPayload(
        CalorimetricData calorimetricData
    )
        : base(calorimetricData)
    {
    }

    public CreateCalorimetricDataPayload(
        CreateCalorimetricDataError error
    )
        : base(error)
    {
    }

    public CreateCalorimetricDataPayload(
        CalorimetricData calorimetricData,
        CreateCalorimetricDataError error
    )
        : base(calorimetricData, error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateCalorimetricDataMutation
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateCalorimetricDataPayload> CreateCalorimetricDataAsync(
        CreateCalorimetricDataInput input,
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken);
        if (currentUser is null)
        {
            return new CreateCalorimetricDataPayload(
                new CreateCalorimetricDataError(
                    CreateCalorimetricDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateCalorimetricDataPayload(
                new CreateCalorimetricDataError(
                    CreateCalorimetricDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create calorimetric data in this database.",
                    []
                )
            );
        }

        var calorimetricData = input.ToDomainModel(currentUser.Uuid);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            calorimetricData.Approval = await responseApprovalService.CreateResponseApproval(calorimetricData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(calorimetricData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateCalorimetricDataPayload(
                calorimetricData,
                new CreateCalorimetricDataError(
                    CreateCalorimetricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreateCalorimetricDataPayload(calorimetricData);
    }
}