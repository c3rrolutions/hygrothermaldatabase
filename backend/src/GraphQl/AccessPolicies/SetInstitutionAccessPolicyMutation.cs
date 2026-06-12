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
using Microsoft.EntityFrameworkCore;
using GraphQL.Client.Abstractions.Utilities;

namespace Database.GraphQl.AccessPolicies;

public sealed record SetInstitutionAccessPolicyInput
(
    DataReferenceInput? Data,
    Guid InstitutionId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
);

[SuppressMessage("Naming", "CA1707")]
public enum SetInstitutionAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_INSTITUTION
}

public sealed record SetInstitutionAccessPolicyError(
    SetInstitutionAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetInstitutionAccessPolicyErrorCode>(Code, Message, Path);

public sealed record SetInstitutionAccessPolicyPayload(
   InstitutionAccessPolicy? InstitutionAccessPolicy,
   IReadOnlyCollection<SetInstitutionAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetInstitutionAccessPolicyMutation
: DataMutationBase<InstitutionAccessPolicy, SetInstitutionAccessPolicyPayload, SetInstitutionAccessPolicyError, SetInstitutionAccessPolicyErrorCode>
{
    protected override SetInstitutionAccessPolicyPayload NewPayload(
        InstitutionAccessPolicy? data,
        IReadOnlyCollection<SetInstitutionAccessPolicyError>? errors
    ) => new(data, errors);

    protected override SetInstitutionAccessPolicyError NewError(
        SetInstitutionAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetInstitutionAccessPolicyPayload> SetInstitutionAccessPolicyAsync(
        SetInstitutionAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetInstitutionAccessPolicyErrorCode.UNAUTHENTICATED,
                SetInstitutionAccessPolicyErrorCode.UNAUTHORIZED,
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
                    SetInstitutionAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var _, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        List<SetInstitutionAccessPolicyError> errors = [];
        if (await institutionByIdDataLoader.LoadAsync(input.InstitutionId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetInstitutionAccessPolicyErrorCode.UNKNOWN_INSTITUTION,
                    $"The institution does not exist.",
                    [nameof(input), nameof(input.InstitutionId).ToLowerFirst()]
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
        var institutionAccessPolicy = await context.InstitutionAccessPolicies
            .SingleOrDefaultAsync(_ =>
                _.DataAccessPolicyId == dataAccessPolicy.Id
                && _.InstitutionId == input.InstitutionId,
                cancellationToken
            );
        if (institutionAccessPolicy is null)
        {
            institutionAccessPolicy = new InstitutionAccessPolicy(dataAccessPolicy.Id, input.InstitutionId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            context.InstitutionAccessPolicies.Add(institutionAccessPolicy);
        }
        else
        {
            institutionAccessPolicy.UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel();
            institutionAccessPolicy.AccessCountSinceStartTime = null;
        }
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(institutionAccessPolicy, null);
    }
}
