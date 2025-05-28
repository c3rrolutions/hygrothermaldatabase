using System;
using Database.ApiRequests.Dto;

namespace Database.Services.Interfaces;

public interface ICacheService
{
    bool TryGetUser(string token, out CurrentUserDto? cacheUser);

    CurrentUserDto? SetUser(string token, CurrentUserDto? cacheUser);

    uint GetAccessCountForUser(Guid userId);

    uint SetAccessCountForUser(Guid userId, uint count);

    (DateTime StartTime, uint Count) GetOrCreateAccessCountForPeriod(Guid institutionId);

    (DateTime StartTime, uint Count) AddAccessCountToPeriod(Guid institutionId);

    (DateTime StartTime, uint Count) SetNewTimePeriod(Guid institutionId);
}