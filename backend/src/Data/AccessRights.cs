using System;
using System.Collections.Generic;
using Database.Services;

namespace Database.Data;

public class AccessRights
    : Entity
{
    public Guid InstitutionId { get; set; }
    public uint? AllowedUserCount { get; set; }
    public uint? AllowedDatasetsPerTime { get; set; }
    public TimeSpan Period { get; set; }
    public List<Guid> UserOfInstitutionAlreadyAccessed { get; private set; }

    public AccessRights(
        Guid institutionId,
        uint? allowedUserCount,
        uint? allowedDatasetsPerTime,
        TimeSpan period
        )
    {
        InstitutionId = institutionId;
        AllowedUserCount = allowedUserCount;
        AllowedDatasetsPerTime = allowedDatasetsPerTime;
        Period = period;
        UserOfInstitutionAlreadyAccessed = new List<Guid>();
    }

    /// <summary>
    /// Check if dataset is restricted by access rights for institution. Datasets per time period of
    /// max user per institution.
    /// </summary>
    /// <param name="dataItem">      <see cref="IData"/> </param>
    /// <param name="currentUserId"> Id of current user. </param>
    /// <param name="cacheService">  <see cref="ICacheService"/> </param>
    /// <param name="restrictions">  List or restrictions </param>
    /// <returns> </returns>
    internal bool IsDataRestricted(IData dataItem, Guid currentUserId, ICacheService cacheService, out List<string> restrictions)
    {
        bool isRestricted = false;
        restrictions = new List<string>();

        // Check restriction for time period
        if (AllowedDatasetsPerTime is not null)
        {
            var accessesPerPeriod = cacheService.GetOrCreateAccessCountForPerirod(InstitutionId);

            if (accessesPerPeriod.StartTime.Add(Period) < DateTime.Now)
            {
                if (accessesPerPeriod.Count >= AllowedDatasetsPerTime)
                {
                    isRestricted = true;
                    restrictions.Add($"Id: {dataItem.Id} Reason: Maximum amount of allowed datasets for institution {InstitutionId} reached.");
                }
            }
            else
            {
                cacheService.SetNewTimePeriod(InstitutionId);
            }
        }

        // Check restriction for users per institution
        if (this.AllowedUserCount is not null)
        {
            if (UserOfInstitutionAlreadyAccessed.Count >= AllowedUserCount)
            {
                isRestricted = true;
                restrictions.Add($"Id: {dataItem.Id} Reason: Maximum allowed users of institution {InstitutionId} reached.");
            }
        }

        // If restricted add user or count
        if (!isRestricted)
        {
            if (AllowedDatasetsPerTime is not null && AllowedDatasetsPerTime > 0)
            {
                cacheService.AddAccessCountToPeriod(InstitutionId);
            }
            if (AllowedUserCount is not null && AllowedUserCount > 0)
            {
                UserOfInstitutionAlreadyAccessed.Add(currentUserId);
            }
        }

        return isRestricted;
    }
}