using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public sealed class UpdateDatabasePayload
    : DatabasePayload<UpdateDatabaseError>
{
    public UpdateDatabasePayload(
        Database? database,
        IReadOnlyCollection<UpdateDatabaseError>? errors
    )
        : base(database, errors)
    {
    }
}