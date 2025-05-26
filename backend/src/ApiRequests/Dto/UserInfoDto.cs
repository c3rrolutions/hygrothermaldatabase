using System.Collections.Generic;

namespace Database.ApiRequests.Dto;

public sealed record UserInfoDto(
    AddressDto? Address,
    string Email,
    bool EmailVerified,
    string Name,
    string? PhoneNumber,
    bool PhoneNumberVerified,
    IReadOnlyList<string>? Roles,
    string Sub, // Subject
    string? Website
);

public sealed record AddressDto(
    string Formatted
);