using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests.Dto;
using Database.Data;
using Database.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Implementation of <see cref="IAccessRightsService"/>
/// </summary>
/// <param name="context">      <see cref="ApplicationDbContext"/> </param>
/// <param name="userService">  <see cref="IUserService"/> </param>
/// <param name="cacheService"> <see cref="ICacheService"/> </param>
/// <param name="logger">       <see cref="ILogger"/> </param>
public class AccessRightsService(
    ApplicationDbContext context,
    IUserService userService,
    ICacheService cacheService,
    ILogger<IAccessRightsService> logger) : IAccessRightsService
{
    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2201:Keine reservierten Ausnahmetypen auslösen", Justification = "<Ausstehend>")]
    public async Task<IQueryable<T>> ApplyAccessRightsOnData<T>(ICollection<T> data, CancellationToken cancellationToken)
    where T : IData
    {
        List<T> filteredData = new List<T>();
        List<string> restrictions = new List<string>();
        List<Guid> institutions = new List<Guid>();
        List<InstitutionAccessRights> accessRights = new List<InstitutionAccessRights>();

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

        return ProcessData(data, applicationId, institutions, currentUser, alreadyAccessedCount, cacheService, accessRights).AsQueryable<T>();
    }

    /// <inheritdoc/>
    public async Task<T?> ApplyAccessRightsOnData<T>(T data, CancellationToken cancellationToken)
    where T : IData
    {
        var result = await ApplyAccessRightsOnData(new List<T> { data }, cancellationToken).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    private IEnumerable<T> ProcessData<T>(
        ICollection<T> data,
        string applicationId,
        List<Guid> institutions,
        CurrentUserDto currentUser,
        int alreadyAccessedCount,
        ICacheService cacheService,
        List<InstitutionAccessRights> accessRights)
    where T : IData
    {
        string reason = "";
        foreach (T dataItem in data)
        {
            if (dataItem.DataAccessRights.HasRistrictions)
            {
                if (dataItem.IsRestrictedByApplication(applicationId))
                {
                    logger.DataRestriction(dataItem.Id, "Application not allowed.");
                    continue;
                }
                if (dataItem.IsRestrictedByInstitutions(institutions))
                {
                    logger.DataRestriction(dataItem.Id, "Institution not allowed.");
                    continue;
                }
                if (dataItem.IsRestrictedByUser(currentUser.Uuid, alreadyAccessedCount))
                {
                    logger.DataRestriction(dataItem.Id, "Allowed accesses for user reached.");
                    continue;
                }
                if (accessRights.Any(x => x.IsDataRestricted(dataItem, currentUser.Uuid, cacheService, out reason)))
                {
                    logger.DataRestriction(dataItem.Id, reason);
                    continue;
                }

                cacheService.SetAccessCountForUser(currentUser.Uuid, ++alreadyAccessedCount);
            }

            yield return dataItem;
        }
    }
}