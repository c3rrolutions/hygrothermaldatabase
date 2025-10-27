using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotChocolate.Types;
using Database.Authorization;
using Database.Extensions;
using Database.Data;
using Database.Services;
using Database.GraphQl.DataX;

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class OpticalDataMutations
: DataMutationBase<OpticalData, CreateOpticalDataPayload, CreateOpticalDataError, CreateOpticalDataErrorCode>
{
    protected override CreateOpticalDataPayload NewPayload(
        OpticalData? data,
        IReadOnlyCollection<CreateOpticalDataError>? errors
    ) => new(data, errors);

    protected override CreateOpticalDataError NewError(
        CreateOpticalDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
        CreateOpticalDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateOpticalDataErrorCode.UNAUTHENTICATED,
                CreateOpticalDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var opticalData = input.ToDomainModel(currentUser.Uuid);
        context.OpticalData.Add(opticalData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                opticalData,
                CreateOpticalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(opticalData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(opticalData, null);
    }
}