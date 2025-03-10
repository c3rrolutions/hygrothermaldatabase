using System;
using System.Collections.Generic;

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
}