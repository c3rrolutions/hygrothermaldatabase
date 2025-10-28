using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using HotChocolate.Types;
using Database.Authorization;
using Database.Extensions;
using Database.Data;
using Database.Services;
using Database.GraphQl.DataX;
using HotChocolate;
using Database.Enumerations;
using System.Linq;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataInput(
    // TODO Why does specifying the type with an attribute not work here?
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    OpticalComponentType? Type,
    OpticalComponentSubtype? Subtype,
    CoatedSide? CoatedSide,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] NearnormalHemisphericalVisibleTransmittances,
    double[] NearnormalHemisphericalVisibleReflectances,
    double[] NearnormalHemisphericalSolarTransmittances,
    double[] NearnormalHemisphericalSolarReflectances,
    double[] InfraredEmittances,
    double[] ColorRenderingIndices,
    IReadOnlyList<CielabColorInput> CielabColors
)
{
    public OpticalData ToDomainModel(Guid userId)
    {
        var opticalData = new OpticalData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            Type,
            Subtype,
            CoatedSide,
            AppliedMethod.ToDomainModel(),
            NearnormalHemisphericalVisibleTransmittances,
            NearnormalHemisphericalVisibleReflectances,
            NearnormalHemisphericalSolarTransmittances,
            NearnormalHemisphericalSolarReflectances,
            InfraredEmittances,
            ColorRenderingIndices,
            CielabColors.Select(c => c.ToDomainModel()).ToList()
        );
        opticalData.Resources.Add(RootResource.ToDomainModel());
        return opticalData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateOpticalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    CREATING_RESPONSE_APPROVAL_FAILED
}

public sealed record CreateOpticalDataError(
    CreateOpticalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateOpticalDataErrorCode>(Code, Message, Path);

public sealed record CreateOpticalDataPayload(
    OpticalData? OpticalData,
    IReadOnlyCollection<CreateOpticalDataError>? Errors
)
: OpticalDataPayload<CreateOpticalDataError>(OpticalData, Errors);

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateOpticalDataMutation
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