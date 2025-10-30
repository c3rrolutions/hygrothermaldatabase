using System;
using Database.Data;

namespace Database.GraphQl;

public sealed record FileMetaInformationInput(
    string[] Path,
    Guid DataFormatId
)
{
    public FileMetaInformation ToDomainModel()
    {
        return new FileMetaInformation(
            Path,
            DataFormatId
        );
    }
};