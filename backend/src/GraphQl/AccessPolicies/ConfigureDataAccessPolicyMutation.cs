using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extensions;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.AccessPolicy;

public sealed record ConfigureDataAccessPolicyInput
(
    DataReferenceInput? Data,
    LogicalCombinator Combinator
);

[SuppressMessage("Naming", "CA1707")]
public enum ConfigureDataAccessPolicyErrorCode
{
    UNKNOWN,
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA
}

public sealed record ConfigureDataAccessPolicyError(
    ConfigureDataAccessPolicyErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<ConfigureDataAccessPolicyErrorCode>(Code, Message, Path);

public sealed record ConfigureDataAccessPolicyPayload(
   DataAccessPolicy? DataAccessPolicy,
   IReadOnlyCollection<ConfigureDataAccessPolicyError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class ConfigureDataAccessPolicyMutation
: DataMutationBase<DataAccessPolicy, ConfigureDataAccessPolicyPayload, ConfigureDataAccessPolicyError, ConfigureDataAccessPolicyErrorCode>
{
    protected override ConfigureDataAccessPolicyPayload NewPayload(
        DataAccessPolicy? data,
        IReadOnlyCollection<ConfigureDataAccessPolicyError>? errors
    ) => new(data, errors);

    protected override ConfigureDataAccessPolicyError NewError(
        ConfigureDataAccessPolicyErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<ConfigureDataAccessPolicyPayload> ConfigureDataAccessPolicyAsync(
        ConfigureDataAccessPolicyInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                ConfigureDataAccessPolicyErrorCode.UNAUTHENTICATED,
                ConfigureDataAccessPolicyErrorCode.UNAUTHORIZED,
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
                    ConfigureDataAccessPolicyErrorCode.UNKNOWN_DATA,
                    context,
                    cancellationToken
                )
                ).Failed(out var data, out var fetchDataErrorPayload)
            )
            {
                return fetchDataErrorPayload;
            }
        }

        var dataId = input.Data?.DataId;
        var dataAccessPolicy = await context.DataAccessPolicies
            .SingleAsync(_ => _.DataId == dataId, cancellationToken);
        dataAccessPolicy.Configure(input.Combinator);
        await context.SaveChangesAsync(cancellationToken);
        return NewPayload(dataAccessPolicy, null);
    }
}