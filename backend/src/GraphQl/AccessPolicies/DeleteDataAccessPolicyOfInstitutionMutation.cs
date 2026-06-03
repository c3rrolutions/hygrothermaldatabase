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

public sealed record DeleteDataAccessPolicyOfInstitutionInput
(
    Guid DataId,
    DataKind DataKind,
    Guid InstitutionId
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum DeleteDataAccessPolicyOfInstitutionErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    UNKNOWN_INSTITUTION
}

public sealed record DeleteDataAccessPolicyOfInstitutionError(
    DeleteDataAccessPolicyOfInstitutionErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<DeleteDataAccessPolicyOfInstitutionErrorCode>(Code, Message, Path);

public sealed record DeleteDataAccessPolicyOfInstitutionPayload(
    DataAccessPolicy? DataAccessPolicy,
    IReadOnlyCollection<DeleteDataAccessPolicyOfInstitutionError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class DeleteDataAccessPolicyOfInstitutionMutation
: DataMutationBase<DataAccessPolicy, DeleteDataAccessPolicyOfInstitutionPayload, DeleteDataAccessPolicyOfInstitutionError, DeleteDataAccessPolicyOfInstitutionErrorCode>
{
    protected override DeleteDataAccessPolicyOfInstitutionPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<DeleteDataAccessPolicyOfInstitutionError>? errors
    ) => new(data, errors);

    protected override DeleteDataAccessPolicyOfInstitutionError NewError(
        DeleteDataAccessPolicyOfInstitutionErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<DeleteDataAccessPolicyOfInstitutionPayload> DeleteDataAccessPolicyOfInstitutionAsync(
        DeleteDataAccessPolicyOfInstitutionInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                DeleteDataAccessPolicyOfInstitutionErrorCode.UNAUTHENTICATED,
                DeleteDataAccessPolicyOfInstitutionErrorCode.UNAUTHORIZED,
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
                DeleteDataAccessPolicyOfInstitutionErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<DeleteDataAccessPolicyOfInstitutionError> errors = [];
        if (await institutionByIdDataLoader.LoadAsync(input.InstitutionId, cancellationToken) is null)
        {
            errors.Add(
                NewError(
                    DeleteDataAccessPolicyOfInstitutionErrorCode.UNKNOWN_INSTITUTION,
                    $"The institution does not exist.",
                    [nameof(input), nameof(input.InstitutionId).ToLowerFirst()]
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.AccessPolicy?.InstitutionAccessPolicies?.RemoveAll(_ => _.InstitutionId == input.InstitutionId);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.AccessPolicy, null);
    }
}