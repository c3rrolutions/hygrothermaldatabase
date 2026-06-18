using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Data.AccessPolicies;
using Database.Extensions;
using Database.GraphQl.DataX;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using NodaTime;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [property: GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTimeOffset CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] Widths,
    double[] Heights,
    double[] Thicknesses
) : IValidateCreateInput
{
    public GeometricData ToDomainModel(
        Guid? userId,
        string? fileExtension
    )
    {
        var data = new GeometricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            Widths,
            Heights,
            Thicknesses
        );
        data.Resources.Add(RootResource.ToDomainModel(fileExtension));
        data.AccessPolicy = new DataAccessPolicy();
        return data;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateGeometricDataErrorCode
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

public sealed record CreateGeometricDataError(
    CreateGeometricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateGeometricDataErrorCode>(Code, Message, Path);

public sealed record CreateGeometricDataPayload(
    GeometricData? GeometricData,
    IReadOnlyCollection<CreateGeometricDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateGeometricDataMutation
: CreateDataMutationBase<GeometricData, CreateGeometricDataPayload, CreateGeometricDataError, CreateGeometricDataErrorCode>
{
    // [UseUserManager]
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    protected override CreateGeometricDataPayload NewPayload(
        GeometricData? data,
        IReadOnlyCollection<CreateGeometricDataError>? errors
    ) => new(data, errors);

    protected override CreateGeometricDataError NewError(
        CreateGeometricDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.AuthenticatedPolicy)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
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
                CreateGeometricDataErrorCode.UNAUTHENTICATED,
                CreateGeometricDataErrorCode.UNAUTHORIZED,
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
                CreateGeometricDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateGeometricDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateGeometricDataErrorCode.UNKNOWN_CREATOR,
                methodByIdDataLoader,
                CreateGeometricDataErrorCode.UNKNOWN_APPLIED_METHOD,
                dataByDatabaseAndIdAndKindDataLoader,
                CreateGeometricDataErrorCode.UNKNOWN_DATABASE,
                CreateGeometricDataErrorCode.UNKNOWN_DATA,
                dataFormatByIdDataLoader,
                CreateGeometricDataErrorCode.UNKNOWN_DATA_FORMAT,
                clock,
                cancellationToken
            )
            ).Failed(out var dataFormat, out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var geometricData = input.ToDomainModel(
            currentUserOrInstitution.CurrentUser?.Uuid,
            dataFormat.Extension
        );
        context.GeometricData.Add(geometricData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                geometricData,
                CreateGeometricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(geometricData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(geometricData, null);
    }
}
