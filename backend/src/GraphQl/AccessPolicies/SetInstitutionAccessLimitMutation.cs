using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetInstitutionAccessLimitInput
(
    Guid InstitutionId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
);

[SuppressMessage("Naming", "CA1707")]
public enum SetInstitutionAccessLimitErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED
}

public sealed record SetInstitutionAccessLimitError(
    SetInstitutionAccessLimitErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetInstitutionAccessLimitErrorCode>(Code, Message, Path);

public sealed record SetInstitutionAccessLimitPayload(
   InstitutionAccessPolicy? InstitutionAccessPolicy,
   IReadOnlyCollection<SetInstitutionAccessLimitError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetInstitutionAccessLimitMutation
: MutationBase<InstitutionAccessPolicy, SetInstitutionAccessLimitPayload, SetInstitutionAccessLimitError, SetInstitutionAccessLimitErrorCode>
{
    protected override SetInstitutionAccessLimitPayload NewPayload(
        InstitutionAccessPolicy? data,
        IReadOnlyCollection<SetInstitutionAccessLimitError>? errors
    ) => new(data, errors);

    protected override SetInstitutionAccessLimitError NewError(
        SetInstitutionAccessLimitErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetInstitutionAccessLimitPayload> SetInstitutionAccessLimitAsync(
        SetInstitutionAccessLimitInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetInstitutionAccessLimitErrorCode.UNAUTHENTICATED,
                SetInstitutionAccessLimitErrorCode.UNAUTHORIZED,
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
            accessPolicy = new InstitutionAccessPolicy(input.InstitutionId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            context.InstitutionAccessPolicies.Add(accessPolicy);
        }
        else
        {
            accessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            accessPolicy.AccessCountSinceStartTime = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(accessPolicy, null);
    }
}