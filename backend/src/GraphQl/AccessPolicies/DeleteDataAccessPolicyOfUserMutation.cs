using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

public sealed record DeleteDataAccessPolicyOfUserInput
(
    Guid DataId,
    DataKind DataKind,
    Guid UserId
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum DeleteDataAccessPolicyOfUserErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_USER
}

public sealed record DeleteDataAccessPolicyOfUserError(
    DeleteDataAccessPolicyOfUserErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteDataAccessPolicyOfUserErrorCode>(Code, Message, Path);

public sealed record DeleteDataAccessPolicyOfUserPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<DeleteDataAccessPolicyOfUserError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteDataAccessPolicyOfUserMutation
: DataMutationBase<DataAccessPolicy, DeleteDataAccessPolicyOfUserPayload, DeleteDataAccessPolicyOfUserError, DeleteDataAccessPolicyOfUserErrorCode>
{
    protected override DeleteDataAccessPolicyOfUserPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<DeleteDataAccessPolicyOfUserError>? errors
    ) => new(data, errors);

    protected override DeleteDataAccessPolicyOfUserError NewError(
        DeleteDataAccessPolicyOfUserErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteDataAccessPolicyOfUserPayload> DeleteDataAccessPolicyOfUserAsync(
        DeleteDataAccessPolicyOfUserInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IUserByIdDataLoader userByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteDataAccessPolicyOfUserErrorCode.UNAUTHENTICATED,
                DeleteDataAccessPolicyOfUserErrorCode.UNAUTHORIZED,
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
                DeleteDataAccessPolicyOfUserErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<DeleteDataAccessPolicyOfUserError> errors = [];
        if (await userByIdDataLoader.LoadAsync(input.UserId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    DeleteDataAccessPolicyOfUserErrorCode.UNKNOWN_USER,
                    $"The user does not exist.",
                    [nameof(input), nameof(input.UserId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy?.UserAccessPolicies?.RemoveAll(_ => _.UserId == input.UserId);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.AccessPolicy, null);
    }
}