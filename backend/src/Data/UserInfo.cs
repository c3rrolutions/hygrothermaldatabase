using System.Collections.Generic;
using Database.ApiRequests.Dto;

namespace Database.Data;

public sealed class Address
{
    public string Formatted { get; private set; }

    public Address(string formatted)
    {
        Formatted = formatted;
    }
}

public sealed class UserInfo
{
    public Address? Address { get; private set; }
    public string Email { get; private set; }
    public bool EmailVerified { get; private set; }
    public string Name { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool PhoneNumberVerified { get; private set; }
    public IReadOnlyList<string>? Roles { get; private set; }
    public string Sub { get; private set; } // Subject
    public string? Website { get; private set; }

    public UserInfo(
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
        Address = address;
        Email = email;
        EmailVerified = emailVerified;
        Name = name;
        PhoneNumber = phonenumber;
        PhoneNumberVerified = phonenumberVerified;
        Roles = roles;
        Sub = subject;
        Website = website;
    }

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