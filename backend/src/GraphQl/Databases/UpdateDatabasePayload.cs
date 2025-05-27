using System.Collections.Generic;

namespace Database.GraphQl.Databases;

public sealed class UpdateDatabasePayload(
    Database? database,
    IReadOnlyCollection<UpdateDatabaseError>? errors
    )
        : DatabasePayload<UpdateDatabaseError>(database, errors)
{
}