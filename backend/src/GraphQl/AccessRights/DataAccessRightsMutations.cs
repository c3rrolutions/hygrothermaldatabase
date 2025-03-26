using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessRights;

[ExtendObjectType(nameof(Mutation))]
public class DataAccessRightsMutations
{
    public async Task<UpdateDataAccessRightsPayload> UpdateAccessRightsToDataAsync(
        DataAccessRightsInput input,
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
            return new UpdateDataAccessRightsPayload(
                new UpdateDataAccessRightsError(
                    UpdateDataAccessRightsErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        //if (!CommonAuthorization.IsAuthorizedToAddApprovalForInstitution(
        //    currentUser,
        //    input.CreatorId,
        //    appSettings,
        //    httpClientFactory,
        //    httpContextAccessor,
        //    cancellationToken
        //    )
        //)
        //    return new AddDataAccessRightsPayload(
        //        new AddDataAccessRightsError(
        //            AddDataAccessRightsErrorCode.UNAUTHORIZED,
        //            $"The current user is not authorized to approval for the institution.",
        //            []
        //        )
        //    );

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

        //if (!CommonAuthorization.(
        //    currentUser,
        //    input.CreatorId,
        //    appSettings,
        //    httpClientFactory,
        //    httpContextAccessor,
        //    cancellationToken
        //    )
        //)
        //    return new AddDataAccessRightsPayload(
        //        new AddDataAccessRightsError(
        //            AddDataAccessRightsErrorCode.UNAUTHORIZED,
        //            $"The current user is not authorized to approval for the institution.",
        //            []
        //        )
        //    );

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
                input.PeriodInDays);
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

        //if (!CommonAuthorization.(
        //    currentUser,
        //    input.CreatorId,
        //    appSettings,
        //    httpClientFactory,
        //    httpContextAccessor,
        //    cancellationToken
        //    )
        //)
        //    return new AddDataAccessRightsPayload(
        //        new AddDataAccessRightsError(
        //            AddDataAccessRightsErrorCode.UNAUTHORIZED,
        //            $"The current user is not authorized to approval for the institution.",
        //            []
        //        )
        //    );

        var accessRights = await context.AccessRights.FirstOrDefaultAsync(x => x.InstitutionId == input.InstitutionId, cancellationToken).ConfigureAwait(false);

        if (accessRights is not null)
        {
            accessRights.AllowedUserCount = input.AllowedUserCount;
            accessRights.AllowedDatasetsPerTime = input.AllowedDatasetsPerTimeSpan;
            accessRights.Period = TimeSpan.FromDays(input.PeriodInDays);
        }
        else
        {
            return new UpdateAccessRightsPayload(
                new AccessRightsError(
                    AccessRightsErrorCode.UNKNOWN_ACCESSRIGHTS,
                    $"There are no access rights for this institution.",
                    []
                )
            );
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new UpdateAccessRightsPayload(accessRights);
    }
}