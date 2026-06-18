using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Enumerations;
using Database.Extensions;
using Database.GraphQl.DataX;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using NodaTime;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataInput(
    [property: GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTimeOffset CreatedAt,
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
) : IValidateCreateInput
{
    public OpticalData ToDomainModel(
        Guid? userId,
        string? fileExtension
    )
    {
        var data = new OpticalData(
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
        data.Resources.Add(RootResource.ToDomainModel(fileExtension));
        data.AccessPolicy = new DataAccessPolicy();
        data.AccessPolicy = new DataAccessPolicy();
        return data;
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
    UNKNOWN_APPLIED_METHOD,
    UNKNOWN_DATABASE,
    UNKNOWN_DATA,
    UNKNOWN_DATA_FORMAT,
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

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
        CreateOpticalDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        IComponentByIdDataLoader componentByIdDataLoader,
        IInstitutionByIdDataLoader institutionByIdDataLoader,
        IMethodByIdDataLoader methodByIdDataLoader,
        IDataByDatabaseAndIdAndKindDataLoader dataByDatabaseAndIdAndKindDataLoader,
        IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        ResponseApprovalService responseApprovalService,
        IClock clock,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateOpticalDataErrorCode.UNAUTHENTICATED,
                CreateOpticalDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUserOrInstitution, out var authorizeErrorPayload)
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
                methodByIdDataLoader,
                CreateOpticalDataErrorCode.UNKNOWN_APPLIED_METHOD,
                dataByDatabaseAndIdAndKindDataLoader,
                CreateOpticalDataErrorCode.UNKNOWN_DATABASE,
                CreateOpticalDataErrorCode.UNKNOWN_DATA,
                dataFormatByIdDataLoader,
                CreateOpticalDataErrorCode.UNKNOWN_DATA_FORMAT,
                clock,
                cancellationToken
            )
            ).Failed(out var dataFormat, out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var opticalData = input.ToDomainModel(
            currentUserOrInstitution.CurrentUser?.Uuid,
            dataFormat.Extension
        );
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
