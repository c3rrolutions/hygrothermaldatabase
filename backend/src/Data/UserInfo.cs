using System.Collections.Generic;
using Database.ApiRequests.Dto;

namespace Database.Data;

public sealed class Address(string formatted)
{
    public string Formatted { get; private set; } = formatted;
}

public sealed class UserInfo(
    Address? address,
    string email,
    bool emailVerified,
    string name,
    string? phonenumber,
    bool phonenumberVerified,
    IReadOnlyList<string>? roles,
    string subject,
    string? website)
{
    public Address? Address { get; private set; } = address;
    public string Email { get; private set; } = email;
    public bool EmailVerified { get; private set; } = emailVerified;
    public string Name { get; private set; } = name;
    public string? PhoneNumber { get; private set; } = phonenumber;
    public bool PhoneNumberVerified { get; private set; } = phonenumberVerified;
    public IReadOnlyList<string>? Roles { get; private set; } = roles;
    public string Sub { get; private set; } = subject;
    public string? Website { get; private set; } = website;

    public static UserInfo? FromDto(UserInfoDto? userInfoDto)
    {
        return userInfoDto is not null ? new UserInfo(
            userInfoDto.Address is not null ? new Address(userInfoDto.Address.Formatted) : null,
            userInfoDto.Email,
            userInfoDto.EmailVerified,
            userInfoDto.Name,
            userInfoDto.PhoneNumber,
            userInfoDto.PhoneNumberVerified,
            userInfoDto.Roles,
            userInfoDto.Sub,
            userInfoDto.Website) : null;
    }
}