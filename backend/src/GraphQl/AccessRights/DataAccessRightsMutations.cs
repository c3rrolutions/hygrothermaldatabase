using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessRights;

[ExtendObjectType(nameof(Mutation))]
public class DataAccessRightsMutations
{
    public async Task<UpdateDataAccessRightsPayload> UpdateDataAccessRightsAsync(
        DataAccessRightsInput input,
        ApplicationDbContext context,
        IUserService userService,
        IDataService dataService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
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
        if (data == null)
        {
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
            currentUser,
            data.CreatorId,
            cancellationToken
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

        data.DataAccessRights.AllowedInstitutions = input.DataAccessRights.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.DataAccessRights.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.DataAccessRights.AllowedUserAndQuantity;

        if (data.DataAccessRights.AllowedInstitutions?.Count > 0
            || data.DataAccessRights.AllowedApplications?.Count > 0
            || data.DataAccessRights.AllowedUserAndQuantity?.Count > 0)
        {
            data.DataAccess = Enumerations.DataAccessMode.RESTRICTED;
        }
        else
        {
            data.DataAccess = Enumerations.DataAccessMode.UNRESTRICTED;
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateDataAccessRightsPayload(data.DataAccessRights);
    }

    public async Task<AddAccessRightsPayload> AddAccessRightsAsync(
        AccessRightsInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        IDataService dataService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new AddAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
            currentUser,
            input.InstitutionId,
            cancellationToken
            )
        )
        {
            return new AddAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to add access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.AccessRights.FirstOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken).ConfigureAwait(false);

        if (accessRights is not null)
        {
            return new AddAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.ALREADY_EXISTS,
                    $"The access rights for this institution already exist.",
                    []
                )
            );
        }
        else
        {
            accessRights = new Data.AccessRights(
                input.InstitutionId,
                input.AllowedUserCount,
                input.AllowedDatasetsPerTimeSpan,
                TimeSpan.FromDays(input.PeriodInDays));
            context.AccessRights.Add(accessRights);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddAccessRightsPayload(accessRights);
    }

    public async Task<UpdateAccessRightsPayload> UpdateAccessRightsAsync(
        AccessRightsInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        IDataService dataService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new UpdateAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantOfVerifiedInstitution(
            currentUser,
            input.InstitutionId,
            cancellationToken
            )
        )
        {
            return new UpdateAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to update access rights for the institution.",
                    []
                )
            );
        }

        var accessRights = await context.AccessRights.FirstOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken).ConfigureAwait(false);

        if (accessRights is null)
        {
            return new UpdateAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNKNOWN_ACCESSRIGHTS,
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