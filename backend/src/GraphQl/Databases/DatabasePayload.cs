using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public abstract class DatabasePayload<TDatabaseError>
    : Payload
    where TDatabaseError : IUserError
{
    protected DatabasePayload(
        Database? database,
        IReadOnlyCollection<TDatabaseError>? errors
    )
    {
        Database = database;
        Errors = errors;
    }

    public Database? Database { get; }
    public IReadOnlyCollection<TDatabaseError>? Errors { get; }
}