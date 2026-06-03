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

public sealed record SetDataAccessLimitForUserInput
(
    Guid DataId,
    DataKind DataKind,
    Guid UserId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum SetDataAccessLimitForUserErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_USER
}

public sealed record SetDataAccessLimitForUserError(
    SetDataAccessLimitForUserErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetDataAccessLimitForUserErrorCode>(Code, Message, Path);

public sealed record SetDataAccessLimitForUserPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<SetDataAccessLimitForUserError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetDataAccessLimitForUserMutation
: DataMutationBase<DataAccessPolicy, SetDataAccessLimitForUserPayload, SetDataAccessLimitForUserError, SetDataAccessLimitForUserErrorCode>
{
    protected override SetDataAccessLimitForUserPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<SetDataAccessLimitForUserError>? errors
    ) => new(data, errors);

    protected override SetDataAccessLimitForUserError NewError(
        SetDataAccessLimitForUserErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetDataAccessLimitForUserPayload> SetDataAccessLimitForUserAsync(
        SetDataAccessLimitForUserInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IUserByIdDataLoader userByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetDataAccessLimitForUserErrorCode.UNAUTHENTICATED,
                SetDataAccessLimitForUserErrorCode.UNAUTHORIZED,
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
                SetDataAccessLimitForUserErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<SetDataAccessLimitForUserError> errors = [];
        if (await userByIdDataLoader.LoadAsync(input.UserId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetDataAccessLimitForUserErrorCode.UNKNOWN_USER,
                    $"The user does not exist.",
                    [nameof(input), nameof(input.UserId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy ??= new DataAccessPolicy();
        data.AccessPolicy.UserAccessPolicies ??= [];
        var accessPolicy =
            data.AccessPolicy.UserAccessPolicies
            .SingleOrDefault(_ => _.UserId == input.UserId);
        if (accessPolicy is null)
        {
            accessPolicy = new UserAccessPolicy(input.UserId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            data.AccessPolicy.UserAccessPolicies.Add(accessPolicy);
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