using System;
using System.Collections.Generic;
using Database.Services;

namespace Database.Data;

public class AccessRights
    : Entity
{
    public Guid InstitutionId { get; set; }
    public int AllowedUserCount { get; set; }
    public List<Guid> UserOfInstitution { get; set; }
    public int AllowedDatasetsPerTime { get; set; }
    public TimeSpan Period { get; set; }

    public AccessRights(
        Guid institutionId,
        int allowedUserCount,
        int allowedDatasetsPerTime,
        int periodInDays
        )
    {
        InstitutionId = institutionId;
        AllowedUserCount = allowedUserCount;
        AllowedDatasetsPerTime = allowedDatasetsPerTime;
        Period = TimeSpan.FromDays(periodInDays);
        UserOfInstitution = new List<Guid>();
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
        if (this.AllowedDatasetsPerTime > 0)
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
        if (this.AllowedUserCount > 0)
        {
            if (this.UserOfInstitution.Count >= this.AllowedUserCount)
            {
                isRestricted = true;
                restrictions.Add($"Id: {dataItem.Id} Reason: Maximum allowed users of institution {InstitutionId} reached.");
            }
        }

        // If restricted add user or count
        if (!isRestricted)
        {
            if (this.AllowedDatasetsPerTime > 0)
            {
                cacheService.AddAccessCountToPeriod(InstitutionId);
            }
            if (this.AllowedUserCount > 0)
            {
                this.UserOfInstitution.Add(currentUserId);
            }
        }

        return isRestricted;
    }
}