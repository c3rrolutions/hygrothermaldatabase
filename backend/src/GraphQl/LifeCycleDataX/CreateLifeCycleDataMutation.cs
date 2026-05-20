using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authorization;
using Database.Data;
using Database.Extensions;
using Database.GraphQl.DataX;
using Database.GraphQl.Scalars;
using Database.Services;
using HotChocolate;
using HotChocolate.Types;
using NodaTime;

namespace Database.GraphQl.LifeCycleDataX;

public sealed record CreateLifeCycleDataInput(
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
    public LifeCycleData ToDomainModel(
        Guid? userId,
        string? fileExtension
    )
    {
        var data = new LifeCycleData(
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
        return data;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateLifeCycleDataErrorCode
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

public sealed record CreateLifeCycleDataError(
    CreateLifeCycleDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateLifeCycleDataErrorCode>(Code, Message, Path);

public sealed record CreateLifeCycleDataPayload(
    LifeCycleData? LifeCycleData,
    IReadOnlyCollection<CreateLifeCycleDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateLifeCycleDataMutation
: CreateDataMutationBase<LifeCycleData, CreateLifeCycleDataPayload, CreateLifeCycleDataError, CreateLifeCycleDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateLifeCycleDataPayload NewPayload(
        LifeCycleData? data,
        IReadOnlyCollection<CreateLifeCycleDataError>? errors
    ) => new(data, errors);

    protected override CreateLifeCycleDataError NewError(
        CreateLifeCycleDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreateLifeCycleDataPayload> CreateLifeCycleDataAsync(
        CreateLifeCycleDataInput input,
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
                CreateLifeCycleDataErrorCode.UNAUTHENTICATED,
                CreateLifeCycleDataErrorCode.UNAUTHORIZED,
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
                CreateLifeCycleDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateLifeCycleDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateLifeCycleDataErrorCode.UNKNOWN_CREATOR,
                methodByIdDataLoader,
                CreateLifeCycleDataErrorCode.UNKNOWN_APPLIED_METHOD,
                dataByDatabaseAndIdAndKindDataLoader,
                CreateLifeCycleDataErrorCode.UNKNOWN_DATABASE,
                CreateLifeCycleDataErrorCode.UNKNOWN_DATA,
                dataFormatByIdDataLoader,
                CreateLifeCycleDataErrorCode.UNKNOWN_DATA_FORMAT,
                clock,
                cancellationToken
            )
            ).Failed(out var dataFormat, out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var lifeCycleData = input.ToDomainModel(
            currentUserOrInstitution.CurrentUser?.Uuid,
            dataFormat.Extension
        );
        context.LifeCycleData.Add(lifeCycleData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                lifeCycleData,
                CreateLifeCycleDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(lifeCycleData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(lifeCycleData, null);
    }
}