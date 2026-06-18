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

namespace Database.GraphQl.HygrothermalDataX;

public sealed record CreateHygrothermalDataInput(
    [property: GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTimeOffset CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource
) : IValidateCreateInput
{
    public HygrothermalData ToDomainModel(
        Guid? userId,
        string? fileExtension
    )
    {
        var data = new HygrothermalData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel()
        );
        data.Resources.Add(RootResource.ToDomainModel(fileExtension));
        data.AccessPolicy = new DataAccessPolicy();
        return data;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateHygrothermalDataErrorCode
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

public sealed record CreateHygrothermalDataError(
    CreateHygrothermalDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateHygrothermalDataErrorCode>(Code, Message, Path);

public sealed record CreateHygrothermalDataPayload(
    HygrothermalData? HygrothermalData,
    IReadOnlyCollection<CreateHygrothermalDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateHygrothermalDataMutation
: CreateDataMutationBase<HygrothermalData, CreateHygrothermalDataPayload, CreateHygrothermalDataError, CreateHygrothermalDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateHygrothermalDataPayload NewPayload(
        HygrothermalData? data,
        IReadOnlyCollection<CreateHygrothermalDataError>? errors
    ) => new(data, errors);

    protected override CreateHygrothermalDataError NewError(
        CreateHygrothermalDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    [Authorize(Policy = AuthorizationPolicies.WriteScopePolicy)]
    public async Task<CreateHygrothermalDataPayload> CreateHygrothermalDataAsync(
        CreateHygrothermalDataInput input,
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
                CreateHygrothermalDataErrorCode.UNAUTHENTICATED,
                CreateHygrothermalDataErrorCode.UNAUTHORIZED,
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
                CreateHygrothermalDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_CREATOR,
                methodByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_APPLIED_METHOD,
                dataByDatabaseAndIdAndKindDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_DATABASE,
                CreateHygrothermalDataErrorCode.UNKNOWN_DATA,
                dataFormatByIdDataLoader,
                CreateHygrothermalDataErrorCode.UNKNOWN_DATA_FORMAT,
                clock,
                cancellationToken
            )
            ).Failed(out var dataFormat, out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var hygrothermalData = input.ToDomainModel(
            currentUserOrInstitution.CurrentUser?.Uuid,
            dataFormat.Extension
        );
        context.HygrothermalData.Add(hygrothermalData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                hygrothermalData,
                CreateHygrothermalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(hygrothermalData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(hygrothermalData, null);
    }
}
