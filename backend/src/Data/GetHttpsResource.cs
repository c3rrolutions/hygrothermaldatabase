using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Database.Extensions;
using Database.Utilities;

namespace Database.Data;

public sealed class GetHttpsResource(
    string? description,
    string hashValue,
    Guid dataFormatId,
    Guid? parentId
    )
        : Entity
{
    public const string FilesDirectoryPath = "./files/";

    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        Guid? calorimetricDataId,
        Guid? geometricDataId,
        Guid? hygrothermalDataId,
        Guid? opticalDataId,
        Guid? photovoltaicDataId,
        Guid? parentId,
        ICollection<FileMetaInformation> archivedFilesMetaInformation,
        ToTreeVertexAppliedConversionMethod? appliedConversionMethod
    )
        : this(
            description,
            hashValue,
            dataFormatId,
            parentId,
            archivedFilesMetaInformation,
            appliedConversionMethod
        )
    {
        CalorimetricDataId = calorimetricDataId;
        GeometricDataId = geometricDataId;
        HygrothermalDataId = hygrothermalDataId;
        OpticalDataId = opticalDataId;
        PhotovoltaicDataId = photovoltaicDataId;
    }

    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        Guid? parentId,
        ICollection<FileMetaInformation> archivedFilesMetaInformation,
        ToTreeVertexAppliedConversionMethod? appliedConversionMethod
    )
        : this(
            description,
            hashValue,
            dataFormatId,
            parentId
        )
    {
        ArchivedFilesMetaInformation = archivedFilesMetaInformation;
        AppliedConversionMethod = appliedConversionMethod;
    }

    // `DbContext` needs this constructor without owned entities.
    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        Guid? calorimetricDataId,
        Guid? hygrothermalDataId,
        Guid? opticalDataId,
        Guid? photovoltaicDataId,
        Guid? geometricDataId,
        Guid? parentId
    )
        : this(
            description,
            hashValue,
            dataFormatId,
            parentId
        )
    {
        CalorimetricDataId = calorimetricDataId;
        HygrothermalDataId = hygrothermalDataId;
        OpticalDataId = opticalDataId;
        PhotovoltaicDataId = photovoltaicDataId;
        GeometricDataId = geometricDataId;
    }

    public string FilePath { get => Path.Combine(FilesDirectoryPath, Id.ToString("D")); }
    public string? Description { get; private set; } = description;
    public string HashValue { get; private set; } = hashValue;
    public Guid DataFormatId { get; private set; } = dataFormatId;

    public ICollection<FileMetaInformation> ArchivedFilesMetaInformation { get; private set; } =
        [];

    // TODO Make sure that at least one ID is always present. In that case `Guid.Empty` should never be used!
    [NotMapped]
    public Guid DataId => CalorimetricDataId ?? HygrothermalDataId ?? OpticalDataId ?? PhotovoltaicDataId ?? GeometricDataId ?? Guid.Empty;

    [NotMapped] public IData? Data => CalorimetricData ?? HygrothermalData ?? OpticalData ?? GeometricData ?? PhotovoltaicData as IData;

    public Guid? CalorimetricDataId { get; private set; }

    [InverseProperty(nameof(CalorimetricData.Resources))]
    public CalorimetricData? CalorimetricData { get; set; }

    public Guid? HygrothermalDataId { get; private set; }

    [InverseProperty(nameof(HygrothermalData.Resources))]
    public HygrothermalData? HygrothermalData { get; set; }

    public Guid? OpticalDataId { get; private set; }

    [InverseProperty(nameof(OpticalData.Resources))]
    public OpticalData? OpticalData { get; set; }

    public Guid? PhotovoltaicDataId { get; private set; }

    [InverseProperty(nameof(PhotovoltaicData.Resources))]
    public PhotovoltaicData? PhotovoltaicData { get; set; }

    public Guid? GeometricDataId { get; private set; }

    [InverseProperty(nameof(GeometricData.Resources))]
    public GeometricData? GeometricData { get; set; }

    public Guid? ParentId { get; private set; } = parentId;

    // TODO Require the conversion method to be given whenever there is a parent. In other words, either both are `null` or both are non-`null`.
    public ToTreeVertexAppliedConversionMethod? AppliedConversionMethod { get; private set; }

    // TODO The parent's `Data` must be the same as this resource's `Data`.
    [InverseProperty(nameof(Children))] public GetHttpsResource? Parent { get; set; }

    [InverseProperty(nameof(Parent))]
    public ICollection<GetHttpsResource> Children { get; } = [];

    public static string ConstructVertexId(Guid id)
    {
        return id.ToString("D").Base64Encode();
    }

    public async Task RecomputeHashValue(CancellationToken cancellationToken)
    {
        HashValue = await Sha256FileHasher.Compute(FilePath, cancellationToken);
    }
}