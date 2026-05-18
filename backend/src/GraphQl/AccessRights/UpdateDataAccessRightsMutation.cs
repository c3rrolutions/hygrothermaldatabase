using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using GraphQL.Client.Abstractions.Utilities;
using HotChocolate.Types;

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
    UNKNOWN_DATA,
    UNKNOWN_INSTITUTION,
    UNKNOWN_USER,
    UNKNOWN_APPLICATION
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
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        IOpenIdConnectApplicationByClientIdDataLoader openIdConnectApplicationByIdDataLoader,
        IUserByIdDataLoader userByIdDataLoader,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                UpdateDataAccessRightsErrorCode.UNAUTHENTICATED,
                UpdateDataAccessRightsErrorCode.UNAUTHORIZED,
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
                UpdateDataAccessRightsErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        List<UpdateDataAccessRightsError> errors = [];

        var unknownInstitutions =
            input.AllowedInstitutions is null
            ? null
            : input.AllowedInstitutions.Zip(
                    await Task.WhenAll(
                        input.AllowedInstitutions
                        .Select(id => institutionByIdDataLoader.LoadAsync(id, cancellationToken))
                    )
                )
                .Where(_ => _.Second is null)
                .Select(_ => _.First)
                .ToList().AsReadOnly();
        if (unknownInstitutions is not null && unknownInstitutions.Count > 0)
        {
            errors.Add(
                NewError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_INSTITUTION,
                    $"The institution(s) '{string.Join("', '", unknownInstitutions)}' do(es) not exist.",
                    [nameof(input), nameof(input.AllowedInstitutions).ToLowerFirst()]
                )
            );
        }

        var unknownOpenIdConnectApplications =
            input.AllowedApplications is null
            ? null
            : input.AllowedApplications.Zip(
                    await Task.WhenAll(
                        input.AllowedApplications
                        .Select(id => openIdConnectApplicationByIdDataLoader.LoadAsync(id, cancellationToken))
                    )
                )
                .Where(_ => _.Second is null)
                .Select(_ => _.First)
                .ToList().AsReadOnly();
        if (unknownOpenIdConnectApplications is not null && unknownOpenIdConnectApplications.Count > 0)
        {
            errors.Add(
                NewError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_APPLICATION,
                    $"The openIdConnectApplication(s) '{string.Join("', '", unknownOpenIdConnectApplications)}' do(es) not exist.",
                    [nameof(input), nameof(input.AllowedApplications).ToLowerFirst()]
                )
            );
        }

        var unknownUsers =
            input.AllowedUserAndQuantity is null
            ? null
            : input.AllowedUserAndQuantity.Zip(
                    await Task.WhenAll(
                        input.AllowedUserAndQuantity
                        .Select(userAndQuantity => userByIdDataLoader.LoadAsync(userAndQuantity.Key, cancellationToken))
                    )
                )
                .Where(_ => _.Second is null)
                .Select(_ => _.First.Key)
                .ToList().AsReadOnly();
        if (unknownUsers is not null && unknownUsers.Count > 0)
        {
            errors.Add(
                NewError(
                    UpdateDataAccessRightsErrorCode.UNKNOWN_USER,
                    $"The user(s) '{string.Join("', '", unknownUsers)}' do(es) not exist.",
                    [nameof(input), nameof(input.AllowedUserAndQuantity).ToLowerFirst()]
                )
            );
        }

        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        data.DataAccessRights.AllowedInstitutions = input.AllowedInstitutions;
        data.DataAccessRights.AllowedApplications = input.AllowedApplications;
        data.DataAccessRights.AllowedUserAndQuantity = input.AllowedUserAndQuantity;

        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(data.DataAccessRights, null);
    }
}