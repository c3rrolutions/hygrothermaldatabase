using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessRights;

[ExtendObjectType(nameof(Mutation))]
public sealed class DataAccessRightsMutations
{
    public async Task<UpdateDataAccessRightsPayload> UpdateDataAccessRightsAsync(
        DataAccessRightsInput input,
        ApplicationDbContext context,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the data.",
                    []
                )
            );
        }

        var data = await context.GetDataAsync(input.DataId, cancellationToken);
        if (data is null)
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        data.DataAccessRights.AllowedInstitutions = input.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.AllowedUserAndQuantity;

        await context.SaveChangesAsync(cancellationToken);
        return new UpdateDataAccessRightsPayload(data.DataAccessRights);
    }

    public async Task<AddInstitutionAccessRightsPayload> AddAccessRightsAsync(
        InstitutionAccessRightsInput input,
        ApplicationDbContext context,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken);
        if (currentUser is null)
        {
            return new AddInstitutionAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new AddInstitutionAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.SingleOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken);

        if (accessRights is not null)
        {
            return new AddInstitutionAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.ALREADY_EXISTS,
                    $"The access rights for this institution already exist.",
                    []
                )
            );
        }

        accessRights = new Data.InstitutionAccessRights(
            input.InstitutionId,
            input.AllowedUserCount,
            input.AllowedDatasetsPerTimeSpan,
            TimeSpan.FromDays(input.PeriodInDays));
        context.InstitutionAccessRights.Add(accessRights);
        await context.SaveChangesAsync(cancellationToken);
        return new AddInstitutionAccessRightsPayload(accessRights);
    }

    public async Task<UpdateAccessRightsPayload> UpdateAccessRightsAsync(
        InstitutionAccessRightsInput input,
        ApplicationDbContext context,
        UserService userService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken);
        if (currentUser is null)
        {
            return new UpdateAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new UpdateAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.SingleOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken);

        if (accessRights is null)
        {
            return new UpdateAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNKNOWN_ACCESS_RIGHTS,
                    $"There are no access rights for this institution.",
                    []
                )
            );
        }

        accessRights.AllowedUserCount = input.AllowedUserCount;
        accessRights.AllowedDatasetsPerTime = input.AllowedDatasetsPerTimeSpan;
        accessRights.Period = TimeSpan.FromDays(input.PeriodInDays);
        await context.SaveChangesAsync(cancellationToken);
        return new UpdateAccessRightsPayload(accessRights);
    }
}