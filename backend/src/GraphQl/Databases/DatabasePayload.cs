using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public abstract class DatabasePayload<TDatabaseError>(
    Database? database,
    IReadOnlyCollection<TDatabaseError>? errors
    )
    : Payload
    where TDatabaseError : IUserError
{
    public Database? Database { get; } = database;
    public IReadOnlyCollection<TDatabaseError>? Errors { get; } = errors;
}