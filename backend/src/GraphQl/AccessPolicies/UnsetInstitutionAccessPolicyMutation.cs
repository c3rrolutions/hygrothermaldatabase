using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record UnsetInstitutionAccessPolicyInput
(
    DataReferenceInput? Data,
    Guid InstitutionId
);

[SuppressMessage("Naming", "CA1707")]
public enum UnsetInstitutionAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_POLICY
}

public sealed record UnsetInstitutionAccessPolicyError(
    UnsetInstitutionAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UnsetInstitutionAccessPolicyErrorCode>(Code, Message, Path);

public sealed record UnsetInstitutionAccessPolicyPayload(
    InstitutionAccessPolicy? InstitutionAccessPolicy,
    IReadOnlyCollection<UnsetInstitutionAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UnsetInstitutionAccessPolicyMutation
: DataMutationBase<InstitutionAccessPolicy, UnsetInstitutionAccessPolicyPayload, UnsetInstitutionAccessPolicyError, UnsetInstitutionAccessPolicyErrorCode>
{
    protected override UnsetInstitutionAccessPolicyPayload NewPayload(
        InstitutionAccessPolicy? data,
        IReadOnlyCollection<UnsetInstitutionAccessPolicyError>? errors
    ) => new(data, errors);

    protected override UnsetInstitutionAccessPolicyError NewError(
        UnsetInstitutionAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<UnsetInstitutionAccessPolicyPayload> UnsetInstitutionAccessPolicyAsync(
        UnsetInstitutionAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UnsetInstitutionAccessPolicyErrorCode.UNAUTHENTICATED,
                UnsetInstitutionAccessPolicyErrorCode.UNAUTHORIZED,
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
                    UnsetInstitutionAccessPolicyErrorCode.UNKNOWN_DATA,
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
        var institutionAccessPolicy = await context.InstitutionAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicy != null
                && _.DataAccessPolicy.DataId == dataId
                && _.InstitutionId == input.InstitutionId,
                cancellationToken
            );
        if (institutionAccessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    UnsetInstitutionAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The institution access policy does not exist.",
                    [nameof(input), nameof(input.InstitutionId).ToLowerFirst()]
                )]
            );
        }

        context.InstitutionAccessPolicies.Remove(institutionAccessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(institutionAccessPolicy, null);
    }
}
