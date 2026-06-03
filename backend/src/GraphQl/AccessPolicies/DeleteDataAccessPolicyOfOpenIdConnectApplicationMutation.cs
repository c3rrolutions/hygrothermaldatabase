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

public sealed record DeleteDataAccessPolicyOfOpenIdConnectApplicationInput
(
    Guid DataId,
    DataKind DataKind,
    string ClientId
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_APPLICATION
}

public sealed record DeleteDataAccessPolicyOfOpenIdConnectApplicationError(
    DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode>(Code, Message, Path);

public sealed record DeleteDataAccessPolicyOfOpenIdConnectApplicationPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<DeleteDataAccessPolicyOfOpenIdConnectApplicationError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteDataAccessPolicyOfOpenIdConnectApplicationMutation
: DataMutationBase<DataAccessPolicy, DeleteDataAccessPolicyOfOpenIdConnectApplicationPayload, DeleteDataAccessPolicyOfOpenIdConnectApplicationError, DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode>
{
    protected override DeleteDataAccessPolicyOfOpenIdConnectApplicationPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<DeleteDataAccessPolicyOfOpenIdConnectApplicationError>? errors
    ) => new(data, errors);

    protected override DeleteDataAccessPolicyOfOpenIdConnectApplicationError NewError(
        DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteDataAccessPolicyOfOpenIdConnectApplicationPayload> DeleteDataAccessPolicyOfOpenIdConnectApplicationAsync(
        DeleteDataAccessPolicyOfOpenIdConnectApplicationInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IOpenIdConnectApplicationByClientIdDataLoader openIdConnectApplicationByClientIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode.UNAUTHENTICATED,
                DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode.UNAUTHORIZED,
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
                DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<DeleteDataAccessPolicyOfOpenIdConnectApplicationError> errors = [];
        if (await openIdConnectApplicationByClientIdDataLoader.LoadAsync(input.ClientId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    DeleteDataAccessPolicyOfOpenIdConnectApplicationErrorCode.UNKNOWN_APPLICATION,
                    $"The Open ID Connect application does not exist.",
                    [nameof(input), nameof(input.ClientId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy?.OpenIdConnectApplicationAccessPolicies?.RemoveAll(_ => _.ClientId == input.ClientId);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.AccessPolicy, null);
    }
}