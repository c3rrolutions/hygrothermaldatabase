using System;
using Database.ApiRequests.Dto;

namespace Database.Services;

public interface ICacheService
{
    bool TryGetUser(string token, out CurrentUserDto? cacheUser);

    CurrentUserDto? SetUser(string token, CurrentUserDto? cacheUser);

    int GetAccessCountForUser(Guid userId);

    int SetAccessCountForUser(Guid userId, int count);

    (DateTime StartTime, int Count) GetOrCreateAccessCountForPerirod(Guid institutionId);

    (DateTime StartTime, int Count) AddAccessCountToPeriod(Guid institutionId);

    (DateTime StartTime, int Count) SetNewTimePeriod(Guid institutionId);
}