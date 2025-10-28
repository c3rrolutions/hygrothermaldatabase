using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;
using System.Diagnostics.CodeAnalysis;
using HotChocolate;
using System.Collections.Generic;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] Thicknesses
)
{
    public GeometricData ToDomainModel(Guid userId)
    {
        var geometricData = new GeometricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            Thicknesses
        );
        geometricData.Resources.Add(RootResource.ToDomainModel());
        return geometricData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateGeometricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateGeometricDataError(
    CreateGeometricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateGeometricDataErrorCode>(Code, Message, Path);

public sealed class CreateGeometricDataPayload
    : GeometricDataPayload<CreateGeometricDataError>
{
    public CreateGeometricDataPayload(
        GeometricData geometricData
    )
        : base(geometricData)
    {
    }

    public CreateGeometricDataPayload(
        CreateGeometricDataError error
    )
        : base(error)
    {
    }

    public CreateGeometricDataPayload(
        GeometricData geometricData,
        CreateGeometricDataError error
    )
        : base(geometricData, error)
    {
    }
}

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateGeometricDataMutation
{
    // [UseUserManager]
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        UserService userService,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new CreateGeometricDataPayload(
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!authorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateGeometricDataPayload(
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create geometric data in this database.",
                    []
                )
            );
        }

        var geometricData = input.ToDomainModel(currentUser.Uuid);
        context.GeometricData.Add(geometricData);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            geometricData.Approval = await responseApprovalService.CreateResponseApproval(geometricData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(geometricData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateGeometricDataPayload(
                geometricData,
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }

        return new CreateGeometricDataPayload(geometricData);
    }
}