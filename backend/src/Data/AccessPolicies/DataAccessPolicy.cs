using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Database.ApiRequests;
using Database.Enumerations;
using Database.Extensions;
using EntityFrameworkCore.Projectables;

namespace Database.Data.AccessPolicies;

public sealed class DataAccessPolicy()
    : AuditableEntity
{
    public const string TableName = "data_access_policy";

    public const string GlobalPolicyCannotBeDeletedTriggerName = $"{TableName}_global_policy_cannot_be_deleted";
    public const string CanOnlyBeDeletedAlongsideCorrespondingDataTriggerName = $"{TableName}_can_only_be_deleted_after_data";
    public const string DataIdCannotChangeTriggerName = $"{TableName}_data_id_cannot_change";
    public static readonly ImmutableArray<string> TriggerNames = [
        GlobalPolicyCannotBeDeletedTriggerName,
        CanOnlyBeDeletedAlongsideCorrespondingDataTriggerName,
        DataIdCannotChangeTriggerName
    ];

    public static readonly ImmutableArray<(string Field, string Table)> DataIdFieldAndDataTableNames = [
        (nameof(CalorimetricDataId), CalorimetricData.TableName),
        (nameof(GeometricDataId), GeometricData.TableName),
        (nameof(HygrothermalDataId), HygrothermalData.TableName),
        (nameof(LifeCycleDataId), LifeCycleData.TableName),
        (nameof(OpticalDataId), OpticalData.TableName),
        (nameof(PhotovoltaicDataId), PhotovoltaicData.TableName)
    ];

    public DataAccessPolicy(
        Guid? calorimetricDataId,
        Guid? geometricDataId,
        Guid? hygrothermalDataId,
        Guid? lifeCycleDataId,
        Guid? opticalDataId,
        Guid? photovoltaicDataId
    )
        : this()
    {
        CalorimetricDataId = calorimetricDataId;
        GeometricDataId = geometricDataId;
        HygrothermalDataId = hygrothermalDataId;
        LifeCycleDataId = lifeCycleDataId;
        OpticalDataId = opticalDataId;
        PhotovoltaicDataId = photovoltaicDataId;
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

    // Note that at least one data ID is always present. So `Guid.Empty` will never be used.
    [NotMapped]
    [Projectable]
    public Guid? DataId => CalorimetricDataId ?? GeometricDataId ?? HygrothermalDataId ?? LifeCycleDataId ?? OpticalDataId ?? PhotovoltaicDataId ?? Guid.Empty;

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

    [InverseProperty(nameof(CalorimetricData.AccessPolicy))]
    public CalorimetricData? CalorimetricData { get; set; }

    public Guid? GeometricDataId { get; private set; }

    [InverseProperty(nameof(GeometricData.AccessPolicy))]
    public GeometricData? GeometricData { get; set; }

    public Guid? HygrothermalDataId { get; private set; }

    [InverseProperty(nameof(HygrothermalData.AccessPolicy))]
    public HygrothermalData? HygrothermalData { get; set; }

    public Guid? LifeCycleDataId { get; private set; }

    [InverseProperty(nameof(LifeCycleData.AccessPolicy))]
    public LifeCycleData? LifeCycleData { get; set; }

    public Guid? OpticalDataId { get; private set; }

    [InverseProperty(nameof(OpticalData.AccessPolicy))]
    public OpticalData? OpticalData { get; set; }

    public Guid? PhotovoltaicDataId { get; private set; }

    [InverseProperty(nameof(PhotovoltaicData.AccessPolicy))]
    public PhotovoltaicData? PhotovoltaicData { get; set; }

    public LogicalCombinator Combinator { get; set; } = LogicalCombinator.ALL;

    [InverseProperty(nameof(UserAccessPolicy.DataAccessPolicy))]
    public ICollection<UserAccessPolicy> UserAccessPolicies { get; } = [];

    [InverseProperty(nameof(InstitutionAccessPolicy.DataAccessPolicy))]
    public ICollection<InstitutionAccessPolicy> InstitutionAccessPolicies { get; } = [];

    [InverseProperty(nameof(OpenIdConnectApplicationAccessPolicy.DataAccessPolicy))]
    public ICollection<OpenIdConnectApplicationAccessPolicy> OpenIdConnectApplicationAccessPolicies { get; } = [];

    [Projectable]
    public bool IsAccessAllowed(
        QueryCurrentUserOrInstitution.CurrentUser? currentUser,
        IReadOnlyList<Guid>? institutionIds,
        string? openIdConnectClientId
    ) =>
        (
            Combinator == LogicalCombinator.ALL && (
                !UserAccessPolicies.Any(_ =>
                    currentUser == null || (
                        _.UserId == currentUser.Uuid
                        && !_.IsAccessAllowed
                    )
                )
                &&
                !InstitutionAccessPolicies.Any(_ =>
                    institutionIds == null || (
                        institutionIds.Contains(_.InstitutionId)
                        && !_.IsAccessAllowed
                    )
                )
                &&
                !OpenIdConnectApplicationAccessPolicies.Any(_ =>
                    openIdConnectClientId == null || (
                        _.ClientId == openIdConnectClientId
                        && !_.IsAccessAllowed
                    )
                )
            )
        )
        ||
        (
            Combinator == LogicalCombinator.SOME && (
                UserAccessPolicies.Any(_ =>
                    currentUser != null
                    && _.UserId == currentUser.Uuid
                    && _.IsAccessAllowed
                )
                ||
                InstitutionAccessPolicies.Any(_ =>
                    institutionIds != null
                    && institutionIds.Contains(_.InstitutionId)
                    && _.IsAccessAllowed
                )
                ||
                OpenIdConnectApplicationAccessPolicies.Any(_ =>
                    openIdConnectClientId != null
                    && _.ClientId == openIdConnectClientId
                    && _.IsAccessAllowed
                )
            )
        );

    public void Reset()
    {
        Combinator = LogicalCombinator.ALL;
        UserAccessPolicies.Clear();
        InstitutionAccessPolicies.Clear();
        OpenIdConnectApplicationAccessPolicies.Clear();
    }
}