using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.PhotovoltaicDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class PhotovoltaicDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreatePhotovoltaicDataPayload> CreatePhotovoltaicDataAsync(
        CreatePhotovoltaicDataInput input,
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
            return new CreatePhotovoltaicDataPayload(
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
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