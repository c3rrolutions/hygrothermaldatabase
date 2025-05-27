using System;

namespace Database.GraphQl.Databases;

public sealed class Database(
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
    public Guid Uuid { get; } = uuid;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public Uri Locator { get; } = locator;
    public DatabaseVerificationState VerificationState { get; } = verificationState;

    public string VerificationCode { get; } = verificationCode;

    // public Institution? Operator { get; set; }
    public bool CanCurrentUserUpdateNode { get; } = canCurrentUserUpdateNode;

    public bool CanCurrentUserVerifyNode { get; } = canCurrentUserVerifyNode;

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