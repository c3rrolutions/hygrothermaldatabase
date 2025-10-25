using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Data;
using Database.Services;
using Database.GraphQl.DataX;
using System.Collections.Generic;

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class OpticalDataMutations
: DataMutationsBase
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
        CreateOpticalDataInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        static CreateOpticalDataPayload NewPayload(
            OpticalData? data,
            IReadOnlyCollection<CreateOpticalDataError>? errors
        ) => new(data, errors);
        static CreateOpticalDataError NewError(
            CreateOpticalDataErrorCode code,
            string message,
            IReadOnlyList<string> path
        ) => new(code, message, path);
        return AuthorizeAsync(
            unauthenticatedErrorCode: CreateOpticalDataErrorCode.UNAUTHENTICATED,
            unauthorizedErrorCode: CreateOpticalDataErrorCode.UNAUTHORIZED,
            errors => NewPayload(null, errors),
            NewError,
            userService,
            then: currentUser => ActAndThenCreateResponseApprovalAsync(
                creatingResponseApprovalFailedErrorCode: CreateOpticalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                NewPayload,
                NewError,
                context,
                responseApprovalService,
                act: async () =>
                {
                    var opticalData = new OpticalData(
                        currentUser.Uuid,
                        input.Locale,
                        input.ComponentId,
                        input.Name,
                        input.Description,
                        input.Warnings,
                        input.CreatorId,
                        input.CreatedAt,
                        input.Type,
                        input.Subtype,
                        input.CoatedSide,
                        input.AppliedMethod.ToDomainModel(),
                        input.NearnormalHemisphericalVisibleTransmittances,
                        input.NearnormalHemisphericalVisibleReflectances,
                        input.NearnormalHemisphericalSolarTransmittances,
                        input.NearnormalHemisphericalSolarReflectances,
                        input.InfraredEmittances,
                        input.ColorRenderingIndices,
                        input.CielabColors.Select(c => c.ToDomainModel()).ToList()
                        );
                    opticalData.Resources.Add(input.RootResource.ToDomainModel());
                    context.OpticalData.Add(opticalData);
                    await context.SaveChangesAsync(cancellationToken);
                    return opticalData;
                },
                undo: async opticalData =>
                {
                    context.Remove(opticalData);
                    await context.SaveChangesAsync(cancellationToken);
                },
                cancellationToken
            ),
            cancellationToken
        );
    }
}