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
        DataService dataService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
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

        var data = await dataService.GetDataAsync(input.DataId, context, cancellationToken).ConfigureAwait(false);
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

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
            currentUser,
            data.CreatorId
            )
        )
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the data.",
                    []
                )
            );
        }

        data.DataAccessRights.AllowedInstitutions = input.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.AllowedUserAndQuantity;

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
            cancellationToken).ConfigureAwait(false);
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

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
            currentUser,
            input.InstitutionId
            )
        )
        {
            return new AddInstitutionAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.FirstOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken).ConfigureAwait(false);

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
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
            cancellationToken).ConfigureAwait(false);
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

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfVerifiedInstitution(
            currentUser,
            input.InstitutionId
            )
        )
        {
            return new UpdateAccessRightsPayload(
                new InstitutionAccessRightsError(
                    InstitutionAccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.InstitutionAccessRights.FirstOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken).ConfigureAwait(false);

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
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateAccessRightsPayload(accessRights);
    }
}