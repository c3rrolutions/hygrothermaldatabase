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
    public async Task<AddDataAccessRightsPayload> AddAccessRightsToDataAsync(
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
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new AddDataAccessRightsPayload(
                new AddDataAccessRightsError(
                    AddDataAccessRightsErrorCode.UNAUTHENTICATED,
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
            return new AddDataAccessRightsPayload(
                new AddDataAccessRightsError(
                    AddDataAccessRightsErrorCode.UNKNOWN_DATA,
                    $"Unknown data.",
                    []
                )
            );
        }

        data.DataAccess = input.DataAccessMode;
        if (data.DataAccess == Enumerations.DataAccessMode.RESTRICTED)
        {
            data.DataAccessRights.AllowedInstitutions = input.DataAccessRights.AllowedInstitutions;

            data.DataAccessRights.AllowedApplications = input.DataAccessRights.AllowedApplications;

            data.DataAccessRights.AllowedUserAndQuantity = input.DataAccessRights.AllowedUserAndQuantity;
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddDataAccessRightsPayload(data.DataAccessRights);
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
            httpContextAccessor,
            cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
        {
            return new AddAccessRightsPayload(
                new AddAccessRightsError(
                    AddAccessRightsErrorCode.UNAUTHENTICATED,
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
            accessRights.Period = input.Period;
        }
        else
        {
            accessRights = new Data.AccessRights(
                input.InstitutionId,
                input.AllowedUserCount,
                input.AllowedDatasetsPerTimeSpan,
                input.Period);
            context.AccessRights.Add(accessRights);
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddAccessRightsPayload(accessRights);
    }
}