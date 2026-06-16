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

public sealed record ClearInstitutionAccessPoliciesInput
(
    DataReferenceInput? Data
);

[SuppressMessage("Naming", "CA1707")]
public enum ClearInstitutionAccessPoliciesErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ClearInstitutionAccessPoliciesError(
    ClearInstitutionAccessPoliciesErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ClearInstitutionAccessPoliciesErrorCode>(Code, Message, Path);

public sealed record ClearInstitutionAccessPoliciesPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<ClearInstitutionAccessPoliciesError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ClearInstitutionAccessPoliciesMutation
: DataMutationBase<DataAccessPolicy, ClearInstitutionAccessPoliciesPayload, ClearInstitutionAccessPoliciesError, ClearInstitutionAccessPoliciesErrorCode>
{
    protected override ClearInstitutionAccessPoliciesPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<ClearInstitutionAccessPoliciesError>? errors
    ) => new(data, errors);

    protected override ClearInstitutionAccessPoliciesError NewError(
        ClearInstitutionAccessPoliciesErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ClearInstitutionAccessPoliciesPayload> ClearInstitutionAccessPoliciesAsync(
        ClearInstitutionAccessPoliciesInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ClearInstitutionAccessPoliciesErrorCode.UNAUTHENTICATED,
                ClearInstitutionAccessPoliciesErrorCode.UNAUTHORIZED,
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
                    ClearInstitutionAccessPoliciesErrorCode.UNKNOWN_DATA,
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
            .Include(_ => _.InstitutionAccessPolicies)
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        dataAccessPolicy.InstitutionAccessPolicies.Clear();
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(dataAccessPolicy, null);
    }
}