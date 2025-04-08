using System;
using System.Collections.Generic;
using Database.Services;

namespace Database.Data;

public class InstitutionAccessRights
    : Entity
{
    public Guid InstitutionId { get; set; }
    public uint? AllowedUserCount { get; set; }
    public uint? AllowedDatasetsPerTime { get; set; }
    public TimeSpan Period { get; set; }
    public List<Guid> UserAlreadyAccessed { get; private set; }

    public InstitutionAccessRights(
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
        UserAlreadyAccessed = new List<Guid>();
    }

    /// <summary>
    /// Check if dataset is restricted by access rights for institution. Datasets per time period of
    /// max user per institution.
    /// </summary>
    /// <param name="dataItem">      <see cref="IData"/> </param>
    /// <param name="currentUserId"> Id of current user. </param>
    /// <param name="cacheService">  <see cref="ICacheService"/> </param>
    /// <param name="reason">        Reason, why data is restricted. </param>
    /// <returns> </returns>
    internal bool IsDataRestricted(IData dataItem, Guid currentUserId, ICacheService cacheService, out string reason)
    {
        bool isRestricted = false;
        reason = "";

        // Check restriction for time period
        if (AllowedDatasetsPerTime is not null)
        {
            var accessesPerPeriod = cacheService.GetOrCreateAccessCountForPeriod(InstitutionId);

            if (accessesPerPeriod.StartTime.Add(Period) < DateTime.Now)
            {
                if (accessesPerPeriod.Count >= AllowedDatasetsPerTime)
                {
                    isRestricted = true;
                    reason = $"Id: {dataItem.Id} Reason: Maximum amount of allowed datasets for institution {InstitutionId} reached.";
                }
            }
            else
            {
                cacheService.SetNewTimePeriod(InstitutionId);
            }
        }

        // Check restriction for users per institution
        if (AllowedUserCount is not null)
        {
            if (UserAlreadyAccessed.Count >= AllowedUserCount)
            {
                isRestricted = true;
                reason = $"Id: {dataItem.Id} Reason: Maximum allowed users of institution {InstitutionId} reached.";
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
                UserAlreadyAccessed.Add(currentUserId);
            }
        }

        return isRestricted;
    }
}