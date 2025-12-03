using NodaTime;
using System;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class CrossDatabaseDataReference(
    Guid dataId,
    OffsetDateTime dataTimestamp,
    DataKind dataKind,
    Guid databaseId
    )
{
    public Guid DataId { get; private set; } = dataId;
    public OffsetDateTime DataTimestamp { get; private set; } = dataTimestamp;
    public DataKind DataKind { get; private set; } = dataKind;
    public Guid DatabaseId { get; private set; } = databaseId;
}