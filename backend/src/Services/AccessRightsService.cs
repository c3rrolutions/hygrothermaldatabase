using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests.Dto;
using Database.Data;
using Database.Logging;
using Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Services;

/// <summary>
/// Service to check if requested data can be returned regarding access rights.
/// </summary>
/// <param name="context">      <see cref="ApplicationDbContext"/> </param>
/// <param name="userService">  <see cref="UserService"/> </param>
/// <param name="cacheService"> <see cref="CacheService"/> </param>
/// <param name="logger">       <see cref="ILogger"/> </param>
public sealed class AccessRightsService(
    ApplicationDbContext context,
    UserService userService,
    CacheService cacheService,
    ILogger<AccessRightsService> logger)
{
    /// <summary>
    /// Apply access rights to passed data list.
    /// </summary>
    /// <param name="data">              Data to apply acces rights on. </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> List of data that can be returned. </returns>
    public async Task<IQueryable<T>> ApplyAccessRightsOnData<T>(IQueryable<T> data, CancellationToken cancellationToken)
    where T : IData
    {
        var filteredData = new List<T>();
        var restrictions = new List<string>();
        var institutions = new List<Guid>();
        var accessRights = new List<InstitutionAccessRights>();

        var applicationId = userService.GetApplicationIdFromUser() ?? throw new InvalidOperationException("Application identifier could not be acquired.");
        var currentUser = await userService.GetCurrentUser(cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException("Could not get current user!");

        var alreadyAccessedCount = cacheService.GetAccessCountForUser(currentUser.Uuid);
        foreach (var institution in currentUser.RepresentedInstitutions.Edges)
        {
            institutions.Add(institution.Node.Uuid);
            var accessRightOfInstitution = await context.InstitutionAccessRights.FirstOrDefaultAsync(x => x.InstitutionId == institution.Node.Uuid, cancellationToken).ConfigureAwait(false);
            if (accessRightOfInstitution is not null)
            {
                accessRights.Add(accessRightOfInstitution);
            }
        }

        var result = ProcessData(data, applicationId, institutions, currentUser, alreadyAccessedCount, cacheService, accessRights);

        // Save InstitutionAccessRight changes
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result.AsQueryable<T>();
    }

    /// <summary>
    /// Apply access rights on passed data item.
    /// </summary>
    /// <param name="data">              Data to apply acces rights on. </param>
    /// <param name="cancellationToken"> <see cref="CancellationToken"/> </param>
    /// <returns> Data item. </returns>
    public async Task<T?> ApplyAccessRightsOnData<T>(T data, CancellationToken cancellationToken)
    where T : IData
    {
        var result = await ApplyAccessRightsOnData(new List<T> { data }.AsQueryable(), cancellationToken).ConfigureAwait(false);

        return result.FirstOrDefault();
    }

    private IEnumerable<T> ProcessData<T>(
        IQueryable<T> data,
        string applicationId,
        List<Guid> institutions,
        CurrentUserDto currentUser,
        uint alreadyAccessedCount,
        CacheService cacheService,
        List<InstitutionAccessRights> accessRights)
    where T : IData
    {
        var reason = "";
        foreach (var dataItem in data)
        {
            if (dataItem.DataAccessRights.HasRestrictions)
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