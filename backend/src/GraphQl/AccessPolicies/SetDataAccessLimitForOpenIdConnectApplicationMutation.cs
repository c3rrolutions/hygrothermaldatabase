using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetDataAccessLimitForOpenIdConnectApplicationInput
(
    Guid DataId,
    DataKind DataKind,
    string ClientId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum SetDataAccessLimitForOpenIdConnectApplicationErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_APPLICATION
}

public sealed record SetDataAccessLimitForOpenIdConnectApplicationError(
    SetDataAccessLimitForOpenIdConnectApplicationErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetDataAccessLimitForOpenIdConnectApplicationErrorCode>(Code, Message, Path);

public sealed record SetDataAccessLimitForOpenIdConnectApplicationPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<SetDataAccessLimitForOpenIdConnectApplicationError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetDataAccessLimitForOpenIdConnectApplicationMutation
: DataMutationBase<DataAccessPolicy, SetDataAccessLimitForOpenIdConnectApplicationPayload, SetDataAccessLimitForOpenIdConnectApplicationError, SetDataAccessLimitForOpenIdConnectApplicationErrorCode>
{
    protected override SetDataAccessLimitForOpenIdConnectApplicationPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<SetDataAccessLimitForOpenIdConnectApplicationError>? errors
    ) => new(data, errors);

    protected override SetDataAccessLimitForOpenIdConnectApplicationError NewError(
        SetDataAccessLimitForOpenIdConnectApplicationErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetDataAccessLimitForOpenIdConnectApplicationPayload> SetDataAccessLimitForOpenIdConnectApplicationAsync(
        SetDataAccessLimitForOpenIdConnectApplicationInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IOpenIdConnectApplicationByClientIdDataLoader openIdConnectApplicationByClientIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetDataAccessLimitForOpenIdConnectApplicationErrorCode.UNAUTHENTICATED,
                SetDataAccessLimitForOpenIdConnectApplicationErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                SetDataAccessLimitForOpenIdConnectApplicationErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<SetDataAccessLimitForOpenIdConnectApplicationError> errors = [];
        if (await openIdConnectApplicationByClientIdDataLoader.LoadAsync(input.ClientId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetDataAccessLimitForOpenIdConnectApplicationErrorCode.UNKNOWN_APPLICATION,
                    $"The Open ID Connect application does not exist.",
                    [nameof(input), nameof(input.ClientId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy ??= new DataAccessPolicy();
        data.AccessPolicy.OpenIdConnectApplicationAccessPolicies ??= [];
        var accessPolicy =
            data.AccessPolicy.OpenIdConnectApplicationAccessPolicies
            .SingleOrDefault(_ => _.ClientId == input.ClientId);
        if (accessPolicy is null)
        {
            accessPolicy = new OpenIdConnectApplicationAccessPolicy(input.ClientId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            data.AccessPolicy.OpenIdConnectApplicationAccessPolicies.Add(accessPolicy);
        }
        else
        {
            accessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            accessPolicy.AccessCountSinceStartTime = null;
        }

        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.AccessPolicy, null);
    }
}