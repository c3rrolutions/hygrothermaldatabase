using System.Collections.Generic;

namespace Database.ApiRequests.Dto;

public sealed record Address(
    string Formatted
);

public sealed record UserInfoDto(
    Address? Address,
    string Email,
    bool EmailVerified,
    string Name,
    string? PhoneNumber,
    bool PhoneNumberVerified,
    IReadOnlyList<string>? Roles,
    string Sub, // Subject
    string? Website
);