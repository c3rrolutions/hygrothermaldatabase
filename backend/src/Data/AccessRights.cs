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
        TimeSpan period
        )
    {
        InstitutionId = institutionId;
        AllowedUserCount = allowedUserCount;
        AllowedDatasetsPerTime = allowedDatasetsPerTime;
        Period = period;
        UserOfInstitution = new List<Guid>();
    }

    internal bool IsDataRestricted(IData dataItem, Guid currentUserId, ICacheService cacheService, out List<string> errors)
    {
        bool isRestricted = false;
        errors = new List<string>();
        if (this.AllowedDatasetsPerTime > 0)
        {
            var accessesPerPeriod = cacheService.GetOrCreateAccessCountForPerirod(InstitutionId);

            if (accessesPerPeriod.StartTime.Add(Period) < DateTime.Now)
            {
                if (accessesPerPeriod.Count >= AllowedDatasetsPerTime)
                {
                    isRestricted = true;
                    errors.Add($"Id: {dataItem.Id} Reason: Maximum amount of allowed datasets reached.");
                }
            }
            else
            {
                cacheService.SetNewTimePeriod(InstitutionId);
            }
        }

        if (this.AllowedUserCount > 0)
        {
            if (this.UserOfInstitution.Count >= this.AllowedUserCount)
            {
                isRestricted = true;
                errors.Add($"Id: {dataItem.Id} Reason: Maximum allowed users of institution reached.");
            }
        }

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