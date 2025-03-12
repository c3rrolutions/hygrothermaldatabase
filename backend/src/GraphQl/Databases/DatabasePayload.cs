using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public abstract class DatabasePayload<TDatabaseError>
    : Payload
    where TDatabaseError : IUserError
{
    protected DatabasePayload(
        Data.Database? database,
        IReadOnlyCollection<TDatabaseError>? errors
    )
    {
        Database = database;
        Errors = errors;
    }

    public Data.Database? Database { get; }
    public IReadOnlyCollection<TDatabaseError>? Errors { get; }
}