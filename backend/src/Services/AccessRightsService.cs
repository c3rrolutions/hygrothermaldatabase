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
    public async Task<(IEnumerable<IData> Data, IEnumerable<string> Restrictions)> ApplyAccessRightsOnData(IQueryable<IData> data, CancellationToken cancellationToken)
    {
        List<IData> filteredData = new List<IData>();
        List<string> restrictions = new List<string>();
        var applicationId = userService.GetApplicationIdFromUser();
        List<Guid> institutions = new List<Guid>();
        var currentUser = await userService.GetCurrentUser(cancellationToken).ConfigureAwait(false);
        var alreadyAccessedCount = cacheService.GetAccessCountForUser(currentUser.Uuid);

        List<AccessRights> accessRights = new List<AccessRights>();
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
            }

            filteredData.Add(dataItem);
        }

        return (filteredData, restrictions);
    }
}