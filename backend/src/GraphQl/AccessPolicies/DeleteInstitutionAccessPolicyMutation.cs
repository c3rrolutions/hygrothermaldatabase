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
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record DeleteInstitutionAccessPolicyInput
(
    Guid InstitutionId
);

[SuppressMessage("Naming", "CA1707")]
public enum DeleteInstitutionAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_POLICY
}

public sealed record DeleteInstitutionAccessPolicyError(
    DeleteInstitutionAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteInstitutionAccessPolicyErrorCode>(Code, Message, Path);

public sealed record DeleteInstitutionAccessPolicyPayload(
   InstitutionAccessPolicy? InstitutionAccessPolicy,
   IReadOnlyCollection<DeleteInstitutionAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteInstitutionAccessPolicyMutation
: MutationBase<InstitutionAccessPolicy, DeleteInstitutionAccessPolicyPayload, DeleteInstitutionAccessPolicyError, DeleteInstitutionAccessPolicyErrorCode>
{
    protected override DeleteInstitutionAccessPolicyPayload NewPayload(
        InstitutionAccessPolicy? data,
        IReadOnlyCollection<DeleteInstitutionAccessPolicyError>? errors
    ) => new(data, errors);

    protected override DeleteInstitutionAccessPolicyError NewError(
        DeleteInstitutionAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteInstitutionAccessPolicyPayload> DeleteInstitutionAccessPolicyAsync(
        DeleteInstitutionAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteInstitutionAccessPolicyErrorCode.UNAUTHENTICATED,
                DeleteInstitutionAccessPolicyErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var _, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }
        var accessPolicy = await context.InstitutionAccessPolicies
            .SingleOrDefaultAsync(x =>
                x.InstitutionId == input.InstitutionId,
                cancellationToken
            );
        if (accessPolicy is null)
        {
            return NewPayload(
                null,
                [NewError(
                    DeleteInstitutionAccessPolicyErrorCode.UNKNOWN_POLICY,
                    $"The access policy does not exist.",
                    [nameof(input), nameof(input.InstitutionId).ToLowerFirst()]
                )]
            );
        }
        context.InstitutionAccessPolicies.Remove(accessPolicy);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicy, null);
    }
}