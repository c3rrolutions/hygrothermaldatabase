using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Database.Extensions;

using Database.Enumerations;

namespace Database.GraphQl.AccessRights;

public sealed record UpdateDataAccessRightsInput
(
    Guid DataId,
    DataKind DataKind,
    IReadOnlyDictionary<Guid, uint?>? AllowedUserAndQuantity,
    IReadOnlyList<Guid>? AllowedInstitutions,
    IReadOnlyList<string>? AllowedApplications
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum UpdateDataAccessRightsErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record UpdateDataAccessRightsError(
    UpdateDataAccessRightsErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<UpdateDataAccessRightsErrorCode>(Code, Message, Path);

public sealed record UpdateDataAccessRightsPayload(
    DataAccessRights? DataAccessRights,
    IReadOnlyCollection<UpdateDataAccessRightsError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class UpdateDataAccessRightsMutation
: DataMutationBase<DataAccessRights, UpdateDataAccessRightsPayload, UpdateDataAccessRightsError, UpdateDataAccessRightsErrorCode>
{
    protected override UpdateDataAccessRightsPayload NewPayload(
        DataAccessRights? data,
        IReadOnlyCollection<UpdateDataAccessRightsError>? errors
    ) => new(data, errors);

    protected override UpdateDataAccessRightsError NewError(
        UpdateDataAccessRightsErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<UpdateDataAccessRightsPayload> UpdateDataAccessRightsAsync(
        UpdateDataAccessRightsInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateDataAccessRightsErrorCode.UNAUTHENTICATED,
                UpdateDataAccessRightsErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                UpdateDataAccessRightsErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        data.DataAccessRights.AllowedInstitutions = input.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.AllowedUserAndQuantity;

        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.DataAccessRights, null);
    }
}