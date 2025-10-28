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
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource
)
{
    public PhotovoltaicData ToDomainModel(Guid userId)
    {
        var photovoltaicData = new PhotovoltaicData(
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
        photovoltaicData.Resources.Add(RootResource.ToDomainModel());
        return photovoltaicData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreatePhotovoltaicDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreatePhotovoltaicDataError(
    CreatePhotovoltaicDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreatePhotovoltaicDataErrorCode>(Code, Message, Path);

public sealed class CreatePhotovoltaicDataPayload
    : PhotovoltaicDataPayload<CreatePhotovoltaicDataError>
{
    public CreatePhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData
    )
        : base(photovoltaicData)
    {
    }

    public CreatePhotovoltaicDataPayload(
        CreatePhotovoltaicDataError error
    )
        : base(error)
    {
    }

    public CreatePhotovoltaicDataPayload(
        PhotovoltaicData photovoltaicData,
        CreatePhotovoltaicDataError error
    )
        : base(photovoltaicData, error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreatePhotovoltaicDataMutation
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreatePhotovoltaicDataPayload> CreatePhotovoltaicDataAsync(
        CreatePhotovoltaicDataInput input,
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
            return new CreatePhotovoltaicDataPayload(
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreatePhotovoltaicDataPayload(
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create photovoltaic data in this database.",
                    []
                )
            );
        }

        var photovoltaicData = input.ToDomainModel(currentUser.Uuid);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            photovoltaicData.Approval = await responseApprovalService.CreateResponseApproval(photovoltaicData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(photovoltaicData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreatePhotovoltaicDataPayload(
                photovoltaicData,
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreatePhotovoltaicDataPayload(photovoltaicData);
    }
}