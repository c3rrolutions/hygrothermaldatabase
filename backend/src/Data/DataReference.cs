using System;
using Database.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class DataReference(
    Guid dataId,
    DataKind dataKind
)
{
    public Guid DataId { get; private set; } = dataId;
    public DataKind DataKind { get; private set; } = dataKind;
}