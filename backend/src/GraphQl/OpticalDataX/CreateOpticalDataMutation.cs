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
using Database.ApiRequests;

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
    double[] NearnormalHemisphericalVisibleTransmittances,
    double[] NearnormalHemisphericalVisibleReflectances,
    double[] NearnormalHemisphericalSolarTransmittances,
    double[] NearnormalHemisphericalSolarReflectances,
    double[] InfraredEmittances,
    double[] ColorRenderingIndices,
    IReadOnlyList<CielabColorInput> CielabColors
) : IValidateCreateInput
{
    public OpticalData ToDomainModel(Guid? userId)
    {
        return new(
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
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateOpticalDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
    UNKNOWN_COMPONENT,
    ILLEGAL_CREATED_AT,
    UNKNOWN_CREATOR,
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
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateOpticalDataMutation
: CreateDataMutationBase<OpticalData, CreateOpticalDataPayload, CreateOpticalDataError, CreateOpticalDataErrorCode>
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
        IComponentByIdDataLoader componentByIdDataLoader,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
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
            ).Failed(out var currentUserOrApplication, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        if ((await ValidateAsync(
                input,
                CreateOpticalDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateOpticalDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateOpticalDataErrorCode.UNKNOWN_CREATOR,
                cancellationToken
            )
            ).Failed(out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var opticalData = input.ToDomainModel(currentUserOrApplication.CurrentUser?.Uuid);
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