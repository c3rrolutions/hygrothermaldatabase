using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record ClearOpenIdConnectApplicationAccessPoliciesInput
(
    DataReferenceInput? Data
);

[SuppressMessage("Naming", "CA1707")]
public enum ClearOpenIdConnectApplicationAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ClearOpenIdConnectApplicationAccessPoliciesError(
    ClearOpenIdConnectApplicationAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearOpenIdConnectApplicationAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearOpenIdConnectApplicationAccessPoliciesPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<ClearOpenIdConnectApplicationAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearOpenIdConnectApplicationAccessPoliciesMutation
: DataMutationBase<DataAccessPolicy, ClearOpenIdConnectApplicationAccessPoliciesPayload, ClearOpenIdConnectApplicationAccessPoliciesError, ClearOpenIdConnectApplicationAccessPoliciesErrorCode>
{
    protected override ClearOpenIdConnectApplicationAccessPoliciesPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<ClearOpenIdConnectApplicationAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearOpenIdConnectApplicationAccessPoliciesError NewError(
        ClearOpenIdConnectApplicationAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<ClearOpenIdConnectApplicationAccessPoliciesPayload> ClearOpenIdConnectApplicationAccessPoliciesAsync(
        ClearOpenIdConnectApplicationAccessPoliciesInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearOpenIdConnectApplicationAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearOpenIdConnectApplicationAccessPoliciesErrorCode.UNAUTHORIZED,
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
                    ClearOpenIdConnectApplicationAccessPoliciesErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .Include(_ => _.OpenIdConnectApplicationAccessPolicies)
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        dataAccessPolicy.OpenIdConnectApplicationAccessPolicies.Clear();
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(dataAccessPolicy, null);
    }
}
