using System;
using System.Collections.Generic;
using System.Linq;
using Database.Data;
using Database.Utilities;

namespace Database.GraphQl;

public sealed record RootGetHttpsResourceInput(
    string Description,
    Guid DataFormatId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation
)
{
    public GetHttpsResource ToDomainModel(string? fileExtension)
    {
        return new GetHttpsResource(
            Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            DataFormatId,
            fileExtension,
            ArchivedFilesMetaInformation.Select(i => i.ToDomainModel()).ToList()
        );
    }
};