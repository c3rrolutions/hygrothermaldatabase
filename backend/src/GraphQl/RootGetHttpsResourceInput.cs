using System;
using System.Collections.Generic;

namespace Database.GraphQl;

public sealed record RootGetHttpsResourceInput(
    string Description,
    Guid DataFormatId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput? AppliedConversionMethod
);