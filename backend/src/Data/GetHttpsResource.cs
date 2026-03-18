using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Enumerations;
using Database.Extensions;
using Database.Utilities;
using EntityFrameworkCore.Projectables;

namespace Database.Data;

public sealed class GetHttpsResource
: Entity
{
    public const string FilesDirectoryPath = "./files/";
    public const string TableName = "get_https_resource";

    public const string DataIdsMustMatchTriggerName = "data_ids_must_match";
    public const string DataIdCannotChangeTriggerName = "data_id_cannot_change";
    public static readonly ImmutableArray<string> TriggerNames = [
        DataIdsMustMatchTriggerName,
        DataIdCannotChangeTriggerName
    ];

    public static readonly ImmutableArray<string> DataIdFieldNames = [
        nameof(CalorimetricDataId),
        nameof(GeometricDataId),
        nameof(HygrothermalDataId),
        nameof(LifeCycleDataId),
        nameof(OpticalDataId),
        nameof(PhotovoltaicDataId)
    ];

    // Constructor for EF Core because navigation properties cannot be set using a constructor: https://learn.microsoft.com/en-us/ef/core/modeling/constructors#binding-to-mapped-properties
    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        string? fileExtension
    )
    {
        Description = description;
        HashValue = hashValue;
        DataFormatId = dataFormatId;
        FileExtension = fileExtension;
    }

    /// <summary>
    /// Construct a root resource.
    /// </summary>
    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        string? fileExtension,
        ICollection<FileMetaInformation> archivedFilesMetaInformation
    )
        : this(
            description,
            hashValue,
            dataFormatId,
            fileExtension
        )
    {
        ParentId = null;
        ArchivedFilesMetaInformation = archivedFilesMetaInformation;
        AppliedConversionMethod = null;
        // The data ID is set by EF Core.
    }

    /// <summary>
    /// Construct a child resource.
    /// </summary>
    public GetHttpsResource(
        string? description,
        string hashValue,
        Guid dataFormatId,
        string? fileExtension,
        Guid? calorimetricDataId,
        Guid? geometricDataId,
        Guid? hygrothermalDataId,
        Guid? lifeCycleDataId,
        Guid? opticalDataId,
        Guid? photovoltaicDataId,
        Guid parentId,
        ICollection<FileMetaInformation> archivedFilesMetaInformation,
        ToTreeVertexAppliedConversionMethod appliedConversionMethod
    )
        : this(
            description,
            hashValue,
            dataFormatId,
            fileExtension
        )
    {
        CalorimetricDataId = calorimetricDataId;
        GeometricDataId = geometricDataId;
        HygrothermalDataId = hygrothermalDataId;
        LifeCycleDataId = lifeCycleDataId;
        OpticalDataId = opticalDataId;
        PhotovoltaicDataId = photovoltaicDataId;
        ParentId = parentId;
        ArchivedFilesMetaInformation = archivedFilesMetaInformation;
        AppliedConversionMethod = appliedConversionMethod;
        AssertThatExactlyOneDataIdIsNonNull();
    }

    private void AssertThatExactlyOneDataIdIsNonNull()
    {
        var nonNullDataIdCount = new Guid?[] {
            CalorimetricDataId,
            GeometricDataId,
            HygrothermalDataId,
            LifeCycleDataId,
            OpticalDataId,
            PhotovoltaicDataId
        }
        .NotNull()
        .Count();
        if (nonNullDataIdCount is 0)
        {
            throw new InvalidOperationException("All data IDs are null.");
        }
        if (nonNullDataIdCount >= 2)
        {
            throw new InvalidOperationException("There is more than 1 non-null data ID.");
        }
    }

    public string? Description { get; private set; }
    public string HashValue { get; private set; }
    public Guid DataFormatId { get; private set; }
    public string? FileExtension { get; private set; }

    public string FileName =>
        Id.ToString("D")
        + (FileExtension is null ? "" : $".{FileExtension}");

    public string FilePath =>
        Path.Combine(FilesDirectoryPath, FileName);

    public ICollection<FileMetaInformation> ArchivedFilesMetaInformation { get; private set; } = [];

    // Note that at least one data ID is always present. So `Guid.Empty` will never be used.
    [NotMapped]
    public Guid DataId => CalorimetricDataId ?? GeometricDataId ?? HygrothermalDataId ?? LifeCycleDataId ?? OpticalDataId ?? PhotovoltaicDataId ?? Guid.Empty;

    [NotMapped]
    public IData? Data => CalorimetricData ?? GeometricData ?? HygrothermalData ?? LifeCycleData ?? OpticalData ?? PhotovoltaicData as IData;

    [Projectable]
    public Guid? GetDataId(DataKind dataKind) =>
        dataKind switch
        {
            DataKind.CALORIMETRIC_DATA => CalorimetricDataId,
            DataKind.GEOMETRIC_DATA => GeometricDataId,
            DataKind.HYGROTHERMAL_DATA => HygrothermalDataId,
            DataKind.LIFE_CYCLE_DATA => LifeCycleDataId,
            DataKind.OPTICAL_DATA => OpticalDataId,
            DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicDataId,
            _ => null, // throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}"),
        };

    [Projectable]
    public IData? GetData(DataKind dataKind) =>
        dataKind switch
        {
            DataKind.CALORIMETRIC_DATA => CalorimetricData,
            DataKind.GEOMETRIC_DATA => GeometricData,
            DataKind.HYGROTHERMAL_DATA => HygrothermalData,
            DataKind.LIFE_CYCLE_DATA => LifeCycleData,
            DataKind.OPTICAL_DATA => OpticalData,
            DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicData,
            _ => null, //throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}"),
        };

    public Guid? CalorimetricDataId { get; private set; }

    [InverseProperty(nameof(CalorimetricData.Resources))]
    public CalorimetricData? CalorimetricData { get; set; }

    public Guid? GeometricDataId { get; private set; }

    [InverseProperty(nameof(GeometricData.Resources))]
    public GeometricData? GeometricData { get; set; }

    public Guid? HygrothermalDataId { get; private set; }

    [InverseProperty(nameof(HygrothermalData.Resources))]
    public HygrothermalData? HygrothermalData { get; set; }

    public Guid? LifeCycleDataId { get; private set; }

    [InverseProperty(nameof(LifeCycleData.Resources))]
    public LifeCycleData? LifeCycleData { get; set; }

    public Guid? OpticalDataId { get; private set; }

    [InverseProperty(nameof(OpticalData.Resources))]
    public OpticalData? OpticalData { get; set; }

    public Guid? PhotovoltaicDataId { get; private set; }

    [InverseProperty(nameof(PhotovoltaicData.Resources))]
    public PhotovoltaicData? PhotovoltaicData { get; set; }

    public Guid? ParentId { get; private set; }

    public ToTreeVertexAppliedConversionMethod? AppliedConversionMethod { get; private set; }

    // TODO The parent's `Data` must be the same as this resource's `Data`.
    [InverseProperty(nameof(Children))] public GetHttpsResource? Parent { get; set; }

    [InverseProperty(nameof(Parent))]
    public ICollection<GetHttpsResource> Children { get; } = [];

    public void UpdateFileExtension(string? fileExtension)
    {
        var oldFilePath = FilePath;
        FileExtension = fileExtension;
        File.Move(oldFilePath, FilePath);
    }

    public bool DoesFileExist()
    {
        return File.Exists(FilePath);
    }

    public void DeleteFile()
    {
        File.Delete(FilePath);
    }

    public async Task RecomputeHashValue(CancellationToken cancellationToken)
    {
        HashValue = await Sha256FileHasher.ComputeForFile(FilePath, cancellationToken);
    }

    public bool IsRoot()
    {
        return ParentId is null;
    }

    public bool IsChild()
    {
        return ParentId is not null;
    }

    public static string ConstructVertexId(Guid id)
    {
        return id.ToString("D").Base64Encode();
    }

    internal void UpdateRoot(
        string description,
        Guid dataFormatId,
        string? fileExtension,
        ICollection<FileMetaInformation> archivedFilesMetaInformation
    )
    {
        if (ParentId is not null)
        {
            throw new InvalidOperationException($"This resource with ID {Id} is not a root but the child of {ParentId}.");
        }
        Description = description;
        DataFormatId = dataFormatId;
        if (fileExtension != FileExtension)
        {
            UpdateFileExtension(fileExtension);
        }
        ArchivedFilesMetaInformation = archivedFilesMetaInformation;
    }

    internal void UpdateChild(
        string description,
        Guid dataFormatId,
        string? fileExtension,
        FileMetaInformation[] archivedFilesMetaInformation,
        ToTreeVertexAppliedConversionMethod appliedConversionMethod
    )
    {
        if (ParentId is null)
        {
            throw new InvalidOperationException($"This resource with ID {Id} is not a child.");
        }
        Description = description;
        DataFormatId = dataFormatId;
        if (fileExtension != FileExtension)
        {
            UpdateFileExtension(fileExtension);
        }
        ArchivedFilesMetaInformation = archivedFilesMetaInformation;
        AppliedConversionMethod = appliedConversionMethod;
    }

    internal void SetParent(
        Guid parentId,
        ToTreeVertexAppliedConversionMethod appliedConversionMethod
    )
    {
        if (ParentId is null)
        {
            throw new InvalidOperationException($"This resource with ID {Id} is not a child.");
        }
        ParentId = parentId;
        AppliedConversionMethod = appliedConversionMethod;
    }
}