using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class HygrothermalDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateHygrothermalDataPayload> CreateHygrothermalDataAsync(
        CreateHygrothermalDataInput input,
        ApplicationDbContext context,
        UserService userService,
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
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
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