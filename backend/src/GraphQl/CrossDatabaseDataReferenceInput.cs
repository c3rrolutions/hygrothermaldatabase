using NodaTime;
using System;
using Database.Data;
using Database.Enumerations;

namespace Database.GraphQl;

public sealed record CrossDatabaseDataReferenceInput(
    Guid DataId,
    OffsetDateTime DataTimestamp,
    DataKind DataKind,
    Guid DatabaseId
)
{
    public CrossDatabaseDataReference ToDomainModel()
    {
        return new CrossDatabaseDataReference(
            DataId,
            DataTimestamp,
            DataKind,
            DatabaseId
        );
    }
};