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

public sealed record SetDataAccessLimitForInstitutionInput
(
    Guid DataId,
    DataKind DataKind,
    Guid InstitutionId,
    UpperLimitPerDurationInput? UpperAccessLimitPerTimeDuration
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum SetDataAccessLimitForInstitutionErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_INSTITUTION
}

public sealed record SetDataAccessLimitForInstitutionError(
    SetDataAccessLimitForInstitutionErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<SetDataAccessLimitForInstitutionErrorCode>(Code, Message, Path);

public sealed record SetDataAccessLimitForInstitutionPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<SetDataAccessLimitForInstitutionError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class SetDataAccessLimitForInstitutionMutation
: DataMutationBase<DataAccessPolicy, SetDataAccessLimitForInstitutionPayload, SetDataAccessLimitForInstitutionError, SetDataAccessLimitForInstitutionErrorCode>
{
    protected override SetDataAccessLimitForInstitutionPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<SetDataAccessLimitForInstitutionError>? errors
    ) => new(data, errors);

    protected override SetDataAccessLimitForInstitutionError NewError(
        SetDataAccessLimitForInstitutionErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<SetDataAccessLimitForInstitutionPayload> SetDataAccessLimitForInstitutionAsync(
        SetDataAccessLimitForInstitutionInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                SetDataAccessLimitForInstitutionErrorCode.UNAUTHENTICATED,
                SetDataAccessLimitForInstitutionErrorCode.UNAUTHORIZED,
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
                SetDataAccessLimitForInstitutionErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<SetDataAccessLimitForInstitutionError> errors = [];
        if (await institutionByIdDataLoader.LoadAsync(input.InstitutionId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    SetDataAccessLimitForInstitutionErrorCode.UNKNOWN_INSTITUTION,
                    $"The institution does not exist.",
                    [nameof(input), nameof(input.InstitutionId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy ??= new DataAccessPolicy();
        data.AccessPolicy.InstitutionAccessPolicies ??= [];
        var accessPolicy =
            data.AccessPolicy.InstitutionAccessPolicies
            .SingleOrDefault(_ => _.InstitutionId == input.InstitutionId);
        if (accessPolicy is null)
        {
            accessPolicy = new InstitutionAccessPolicy(input.InstitutionId)
            {
                UpperAccessLimitPerTimeDuration = input.UpperAccessLimitPerTimeDuration?.ToDomainModel()
            };
            data.AccessPolicy.InstitutionAccessPolicies.Add(accessPolicy);
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