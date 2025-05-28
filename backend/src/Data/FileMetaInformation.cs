using System;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class FileMetaInformation(
    string[] path,
    Guid dataFormatId
    )
{
    public string[] Path { get; private set; } = path;
    public Guid DataFormatId { get; private set; } = dataFormatId;
}