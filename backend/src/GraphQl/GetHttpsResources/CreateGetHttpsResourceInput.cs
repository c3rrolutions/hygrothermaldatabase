using System;
using System.Collections.Generic;
using Database.Enumerations;

namespace Database.GraphQl.GetHttpsResources;

public sealed record CreateGetHttpsResourceInput(
    string Description,
    Guid DataFormatId,
    Guid DataId,
    DataKind DataKind,
    Guid? ParentId,
    IReadOnlyList<FileMetaInformationInput> ArchivedFilesMetaInformation,
    ToTreeVertexAppliedConversionMethodInput? AppliedConversionMethod
);