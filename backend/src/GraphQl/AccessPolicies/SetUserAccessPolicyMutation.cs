using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using Database.ApiRequests;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using GraphQL.Client.Abstractions.Utilities;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetUserAccessPolicyInput
(
    DataReferenceInput? Data,
    Guid UserId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
);

[SuppressMessage("Naming", "CA1707")]
public enum SetUserAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_USER
}

public sealed record SetUserAccessPolicyError(
    SetUserAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetUserAccessPolicyErrorCode>(Code, Message, Path);

public sealed record SetUserAccessPolicyPayload(
   UserAccessPolicy? UserAccessPolicy,
   IReadOnlyCollection<SetUserAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetUserAccessPolicyMutation
: DataMutationBase<UserAccessPolicy, SetUserAccessPolicyPayload, SetUserAccessPolicyError, SetUserAccessPolicyErrorCode>
{
    protected override SetUserAccessPolicyPayload NewPayload(
        UserAccessPolicy? data,
        IReadOnlyCollection<SetUserAccessPolicyError>? errors
    ) => new(data, errors);

    protected override SetUserAccessPolicyError NewError(
        SetUserAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<SetUserAccessPolicyPayload> SetUserAccessPolicyAsync(
        SetUserAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IUserByIdDataLoader userByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetUserAccessPolicyErrorCode.UNAUTHENTICATED,
                SetUserAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if (input.Data is not null)
        {
            if ((await FetchDataAsync(
                    input.Data,
                    SetUserAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        List<SetUserAccessPolicyError> errors = [];
        if (await userByIdDataLoader.LoadAsync(input.UserId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetUserAccessPolicyErrorCode.UNKNOWN_USER,
                    $"The user does not exist.",
                    [nameof(input), nameof(input.UserId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        var userAccessPolicy = await context.UserAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicyId == dataAccessPolicy.Id
                && _.UserId == input.UserId,
                cancellationToken
            );
        if (userAccessPolicy is null)
        {
            userAccessPolicy = new UserAccessPolicy(dataAccessPolicy.Id, input.UserId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            context.UserAccessPolicies.Add(userAccessPolicy);
        }
        else
        {
            userAccessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            userAccessPolicy.AccessCountSinceStartTime = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(userAccessPolicy, null);
    }
}
