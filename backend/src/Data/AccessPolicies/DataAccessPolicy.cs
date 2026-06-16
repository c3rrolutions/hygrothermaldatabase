using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Database.Enumerations;
using Database.Extensions;
using Database.GraphQl.AccessPolicy;
using EntityFrameworkCore.Projectables;
using HotChocolate;

namespace Database.Data.AccessPolicies;

[GraphQLDescription($"""
A data access policy decides who can access data, meaning which data shows up
in GraphQL queries and which associated resources can be downloaded. The
decision is made based on the authenticated user or institution, the
institutions represented by a user, and/or the communicating OpenID Connect
application. The access token issued by the metabase and associated with
same-site logins or given in the HTTP Authorization header as Bearer, tells
through the 'Subject' claim which user or institution is authenticated and
through the 'Client ID' or 'Authorized Party' claim which OpenID Connect
application is communicating. In the case of a user, the metabase informs us
about the institutions he*she represents.

How user, institution(s), and application are allowed/restricted is decided by
user, institution, and application access policies associated with the data
access policy and specific users, institutions, and applications through their
IDs or users and institutions and client IDs for applications. Each such policy
can allow access to a user, institution, or application and can limit the
number of API accesses totally or within a time span, which is shifted to the
current moment once it has passed. For example, an institution policy without a
limit just allows access for any user representing that institution and for any
application owned by the institution itself, and it disallows access for all
other users and institutions. If an additional limit without duration is given,
than exactly that number of GraphQL or REST API accesses are allowed. And if an
additional duration is given, from the time of the first access until the
duration passed, the given number of accesses are allowed; the start time and
the access count are reset on the first access after the duration passed (a
sliding window of time). Note that if the limit is 0, access is explicitly
denied for the respective user, institution, or application.

The individual decisions based on user, institution, and OpenID Connect
application, can be combined conjuctively ('and' or 'all' need to be positive)
or disjunctively ('or' or 'someone'/'at least one' needs to be positive). This
is configured through the combinator and the mutation
`{nameof(ConfigureDataAccessPolicyMutation)}`. If there are no user access
policies at all, then, in the 'all' case, no restrictions based on the user
itself are imposed, and in the 'some' case, no allowances based on the user
itself are given; and analogously for institution and application policies. Put
another way, an empty list of user access policies is `true` in the 'and' case
and `false` in the 'or' case, and analogously for institution and application
access policies.

In particular, a data access policy with the combinator 'all' and empty user,
institution, and application policies allows access to anyone, also anonymous
access. And one with the combinator 'or' and empty policies allows access to
noone, no matter if authenticated or not.

A data access policy is either the one-and-only global one or associated with a
specific data entry, see the field `{nameof(DataAccessPolicy.Data)}`. It is
global if this field is `null`. The global and individual policies are combined
conjunctively, meaning that for access both need to allow access.

There are mutations to
* reset a data access policy to its original state, in which anyone is allowed
  access;
* configure the logical combinator of a data access policy (as explained above);
* clear user, institution, and applicatoin policies of a data access policy;
* set and unset a user, institution, or applicatoin policy for a specific user,
  institution, or application, and a specific data access policy (global or
  associated with a specific data entry).

To determine whether access policies allow/restrict access in the way you
expected, use their `is*Allowed` fields, passing the applicable user ID,
institution IDs, and/or client ID as argument(s). The field
`{nameof(AccessPolicyBase.AccessCountSinceStartTime)}` informs you about the
number of accesses that were counted for the user, institution, or application
policy since the start time. Both are reset on the first access after the end
of the time span or when a policy is updated via the respective 'set' mutation.

Note that durations are represented in the
[ISO 8601 duration format](https://scalars.graphql.org/chillicream/duration.html#sec-Result-spec)
""")]
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

    // Note that for the global data access policy, all `*DataId`s are null
    [Projectable]
    public Guid? DataId => CalorimetricDataId ?? GeometricDataId ?? HygrothermalDataId ?? LifeCycleDataId ?? OpticalDataId ?? PhotovoltaicDataId ?? null;

    public IData? Data => CalorimetricData ?? GeometricData ?? HygrothermalData ?? LifeCycleData ?? OpticalData ?? PhotovoltaicData as IData;

    [Projectable]
    public Database.Enumerations.DataKind? DataKind =>
        CalorimetricDataId != null ? Database.Enumerations.DataKind.CALORIMETRIC_DATA
        : GeometricDataId != null ? Database.Enumerations.DataKind.GEOMETRIC_DATA
        : HygrothermalDataId != null ? Database.Enumerations.DataKind.HYGROTHERMAL_DATA
        : LifeCycleDataId != null ? Database.Enumerations.DataKind.LIFE_CYCLE_DATA
        : OpticalDataId != null ? Database.Enumerations.DataKind.OPTICAL_DATA
        : PhotovoltaicDataId != null ? Database.Enumerations.DataKind.PHOTOVOLTAIC_DATA
        : null;

    [Projectable]
    public Guid? GetDataId(Database.Enumerations.DataKind dataKind) =>
        dataKind switch
        {
            Database.Enumerations.DataKind.CALORIMETRIC_DATA => CalorimetricDataId,
            Database.Enumerations.DataKind.GEOMETRIC_DATA => GeometricDataId,
            Database.Enumerations.DataKind.HYGROTHERMAL_DATA => HygrothermalDataId,
            Database.Enumerations.DataKind.LIFE_CYCLE_DATA => LifeCycleDataId,
            Database.Enumerations.DataKind.OPTICAL_DATA => OpticalDataId,
            Database.Enumerations.DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicDataId,
            _ => null, // throw new ArgumentOutOfRangeException(nameof(dataKind), $"Unsupported data kind {dataKind}"),
        };

    [Projectable]
    public IData? GetData(Database.Enumerations.DataKind dataKind) =>
        dataKind switch
        {
            Database.Enumerations.DataKind.CALORIMETRIC_DATA => CalorimetricData,
            Database.Enumerations.DataKind.GEOMETRIC_DATA => GeometricData,
            Database.Enumerations.DataKind.HYGROTHERMAL_DATA => HygrothermalData,
            Database.Enumerations.DataKind.LIFE_CYCLE_DATA => LifeCycleData,
            Database.Enumerations.DataKind.OPTICAL_DATA => OpticalData,
            Database.Enumerations.DataKind.PHOTOVOLTAIC_DATA => PhotovoltaicData,
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
    public bool IsGlobal => DataId is null;

    [Projectable]
    public bool IsAnyoneAllowed => IsAccessAllowed(null, null, null);

    [Projectable]
    public bool IsAccessAllowed(
        Guid? userId,
        Guid[]? institutionIds,
        string? openIdConnectClientId
    ) =>
        (
            Combinator == LogicalCombinator.ALL && (
                (
                    UserAccessPolicies.Count == 0
                    || UserAccessPolicies.Any(_ =>
                        _.IsAccessAllowed(userId)
                    )
                )
                &&
                (
                    InstitutionAccessPolicies.Count == 0
                    || InstitutionAccessPolicies.Any(_ =>
                        _.IsAccessAllowed(institutionIds)
                    )
                )
                &&
                (
                    OpenIdConnectApplicationAccessPolicies.Count == 0
                    || OpenIdConnectApplicationAccessPolicies.Any(_ =>
                        _.IsAccessAllowed(openIdConnectClientId)
                    )
                )
            )
        )
        ||
        (
            Combinator == LogicalCombinator.SOME && (
                UserAccessPolicies.Any(_ =>
                    _.IsAccessAllowed(userId)
                )
                ||
                InstitutionAccessPolicies.Any(_ =>
                    _.IsAccessAllowed(institutionIds)
                )
                ||
                OpenIdConnectApplicationAccessPolicies.Any(_ =>
                    _.IsAccessAllowed(openIdConnectClientId)
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

    public void Configure(
        LogicalCombinator combinator
    )
    {
        Combinator = combinator;
    }
}