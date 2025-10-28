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
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource
)
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
        hygrothermalData.Resources.Add(RootResource.ToDomainModel());
        return hygrothermalData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateHygrothermalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateHygrothermalDataError(
    CreateHygrothermalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateHygrothermalDataErrorCode>(Code, Message, Path);

public sealed class CreateHygrothermalDataPayload
    : HygrothermalDataPayload<CreateHygrothermalDataError>
{
    public CreateHygrothermalDataPayload(
        HygrothermalData hygrothermalData
    )
        : base(hygrothermalData)
    {
    }

    public CreateHygrothermalDataPayload(
        CreateHygrothermalDataError error
    )
        : base(error)
    {
    }

    public CreateHygrothermalDataPayload(
        HygrothermalData hygrothermalData,
        CreateHygrothermalDataError error
    )
        : base(hygrothermalData, error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateHygrothermalDataMutation
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateHygrothermalDataPayload> CreateHygrothermalDataAsync(
        CreateHygrothermalDataInput input,
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
            return new CreateHygrothermalDataPayload(
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateHygrothermalDataPayload(
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create hygrothermal data in this database.",
                    []
                )
            );
        }

        var hygrothermalData = input.ToDomainModel(currentUser.Uuid);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            hygrothermalData.Approval = await responseApprovalService.CreateResponseApproval(hygrothermalData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(hygrothermalData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateHygrothermalDataPayload(
                hygrothermalData,
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreateHygrothermalDataPayload(hygrothermalData);
    }
}