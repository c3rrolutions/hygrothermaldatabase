using System;

namespace Database.GraphQl.Databases;

public sealed class Database
{
    public Database(
        Guid uuid,
        string name,
        string description,
        Uri locator,
        DatabaseVerificationState verificationState,
        string verificationCode,
        bool canCurrentUserUpdateNode,
        bool canCurrentUserVerifyNode
    )
    {
        Uuid = uuid;
        Name = name;
        Description = description;
        Locator = locator;
        VerificationState = verificationState;
        VerificationCode = verificationCode;
        CanCurrentUserUpdateNode = canCurrentUserUpdateNode;
        CanCurrentUserVerifyNode = canCurrentUserVerifyNode;
    }

    public Guid Uuid { get; }
    public string Name { get; }
    public string Description { get; }
    public Uri Locator { get; }
    public DatabaseVerificationState VerificationState { get; }

    public string VerificationCode { get; }

    // public Institution? Operator { get; set; }
    public bool CanCurrentUserUpdateNode { get; }

    public bool CanCurrentUserVerifyNode { get; }

    public static Database FromDto(DatabaseDto databaseDto)
    {
        return new Database(
            databaseDto.Uuid,
            databaseDto.Name,
            databaseDto.Description,
            databaseDto.Locator,
            databaseDto.VerificationState,
            databaseDto.VerificationCode,
            databaseDto.CanCurrentUserUpdateNode,
            databaseDto.CanCurrentUserVerifyNode);
    }
}