using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IAccessRightsService"/>
/// </summary>
/// <param name="context">      <see cref="ApplicationDbContext"/> </param>
/// <param name="userService">  <see cref="IUserService"/> </param>
/// <param name="cacheService"> <see cref="ICacheService"/> </param>
public class AccessRightsService(
    ApplicationDbContext context,
    IUserService userService,
    ICacheService cacheService) : IAccessRightsService
{
    /// <inheritdoc/>
    public async Task<(ICollection<IData> Data, ICollection<string> Restrictions)> ApplyAccessRightsOnData(ICollection<IData> data, CancellationToken cancellationToken)
    {
        List<IData> filteredData = new List<IData>();
        List<string> restrictions = new List<string>();
        List<Guid> institutions = new List<Guid>();
        List<AccessRights> accessRights = new List<AccessRights>();

        var applicationId = userService.GetApplicationIdFromUser() ?? throw new Exception("Application Id could not be aquired.");
        var currentUser = await userService.GetCurrentUser(cancellationToken).ConfigureAwait(false) ?? throw new Exception("Could not get current user!");

        var alreadyAccessedCount = cacheService.GetAccessCountForUser(currentUser.Uuid);
        foreach (var institution in currentUser.RepresentedInstitutions.Edges)
        {
            institutions.Add(institution.Node.Uuid);
            var accessRightOfInstitution = await context.AccessRights.FirstOrDefaultAsync(x => x.InstitutionId == institution.Node.Uuid, cancellationToken).ConfigureAwait(false);
            if (accessRightOfInstitution is not null)
            {
                accessRights.Add(accessRightOfInstitution);
            }
        }

        foreach (IData dataItem in data)
        {
            if (dataItem.DataAccess == Enumerations.DataAccessMode.RESTRICTED)
            {
                if (dataItem.IsRestrictedByApplication(applicationId))
                {
                    restrictions.Add($"Restricted Item Id: {dataItem.Id} Reason: Application not allowed.");
                    continue;
                }
                if (dataItem.IsRestrictedByInstitutions(institutions))
                {
                    restrictions.Add($"Restricted Item Id: {dataItem.Id} Reason: Institution not allowed.");
                    continue;
                }
                if (dataItem.IsRestrictedByUser(currentUser.Uuid, alreadyAccessedCount))
                {
                    restrictions.Add($"Restricted Item Id: {dataItem.Id} Reason: Allowed accesses for user reached.");
                    continue;
                }
                if (accessRights.Any(x => x.IsDataRestricted(dataItem, currentUser.Uuid, cacheService, out restrictions)))
                {
                    continue;
                }

                cacheService.SetAccessCountForUser(currentUser.Uuid, ++alreadyAccessedCount);
            }

            filteredData.Add(dataItem);
        }

        return (filteredData, restrictions);
    }

    /// <inheritdoc/>
    public async Task<(IData? Data, string? Restrictions)> ApplyAccessRightsOnData(IData data, CancellationToken cancellationToken)
    {
        var result = await ApplyAccessRightsOnData(new List<IData> { data }, cancellationToken).ConfigureAwait(false);

        return (result.Data.FirstOrDefault(), result.Restrictions.FirstOrDefault());
    }
}