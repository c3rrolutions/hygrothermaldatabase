using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Extensions;
using Database.Services;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace Database.GraphQl.DataX;

public sealed record PublishDataInput(
    Guid DataId,
    DataKind DataKind
) : IIdentifyDataInput;

[SuppressMessage("Naming", "CA1707")]
public enum PublishDataErrorCode
{
    UNAUTHENTICATED,
    UNAUTHORIZED,
    UNKNOWN_DATA,
    CREATING_RESPONSE_APPROVAL_FAILED,
    MISSING_ROOT_RESOURCE,
    DUPLICATE_ROOT_RESOURCE,
    RESOURCES_WITHOUT_FILE
}

public sealed record PublishDataError(
    PublishDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
) : UserErrorBase<PublishDataErrorCode>(Code, Message, Path);

public sealed record PublishDataPayload(
    IData? Data,
    IReadOnlyCollection<PublishDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class PublishDataMutation
: DataMutationBase<IData, PublishDataPayload, PublishDataError, PublishDataErrorCode>
{
    protected override PublishDataPayload NewPayload(
        IData? data,
        IReadOnlyCollection<PublishDataError>? errors
    ) => new(data, errors);

    protected override PublishDataError NewError(
        PublishDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<PublishDataPayload> PublishDataAsync(
        PublishDataInput input,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        ApplicationDbContext context,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                PublishDataErrorCode.UNAUTHENTICATED,
                PublishDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await FetchDataAsync(
                input,
                PublishDataErrorCode.UNKNOWN_DATA,
                context,
                cancellationToken
            )
            ).Failed(out var data, out var fetchDataErrorPayload)
        )
        {
            return fetchDataErrorPayload;
        }

        var errors = new List<PublishDataError>();
        var rootResources =
            await context.GetHttpsResources.AsQueryable()
            .Where(_ => _.ParentId == null)
            .Where(_ => _.GetDataId(input.DataKind) == input.DataId)
            .ToListAsync(cancellationToken);
        if (rootResources.Count is 0)
        {
            errors.Add(
                NewError(
                    PublishDataErrorCode.MISSING_ROOT_RESOURCE,
                    "The data set does not have a root resource.",
                    []
                )
            );
        }
        if (rootResources.Count >= 2)
        {
            errors.Add(
                NewError(
                    PublishDataErrorCode.DUPLICATE_ROOT_RESOURCE,
                    $"The data set has more than one root resource, namely those with the IDs {string.Join(", ", rootResources.Select(_ => _.Id.ToString("D")))}.",
                    []
                )
            );
        }
        var resources =
            await context.GetHttpsResources.AsQueryable()
            .Where(_ => _.GetDataId(input.DataKind) == input.DataId)
            .ToListAsync(cancellationToken);
        var resourcesWithoutFile = resources.Where(_ => !_.DoesFileExist()).ToList();
        if (resourcesWithoutFile.Count > 0)
        {
            errors.Add(
                NewError(
                    PublishDataErrorCode.RESOURCES_WITHOUT_FILE,
                    $"The data set has at least one resource without a file, namely those with the IDs {string.Join(", ", resourcesWithoutFile.Select(_ => _.Id.ToString("D")))}.",
                    []
                )
            );
        }
        if (errors.Count > 0)
        {
            return NewPayload(null, errors);
        }

        var rememberedPublishingState = data.PublishingState;
        data.Publish();
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                data,
                PublishDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            if (rememberedPublishingState is PublishingState.RETRACTED)
            {
                data.Retract();
            }
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(data, null);
    }
}
