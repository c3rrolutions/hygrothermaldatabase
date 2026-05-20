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

namespace Database.GraphQl.CalorimetricDataX;

public sealed record CreateCalorimetricDataInput(
    [property: GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTimeOffset CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] GValues,
    double[] UValues
) : IValidateCreateInput
{
    public CalorimetricData ToDomainModel(
        Guid? userId,
        string? fileExtension
    )
    {
        var data = new CalorimetricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            GValues,
            UValues
        );
        data.Resources.Add(RootResource.ToDomainModel(fileExtension));
        return data;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateCalorimetricDataErrorCode
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

public sealed record CreateCalorimetricDataError(
    CreateCalorimetricDataErrorCode Code,
    string Message,
    IReadOnlyList<string> Path
)
: UserErrorBase<CreateCalorimetricDataErrorCode>(Code, Message, Path);

public sealed record CreateCalorimetricDataPayload(
    CalorimetricData? CalorimetricData,
    IReadOnlyCollection<CreateCalorimetricDataError>? Errors
) : Payload;

[ExtendObjectType(nameof(Mutation))]
public sealed class CreateCalorimetricDataMutation
: CreateDataMutationBase<CalorimetricData, CreateCalorimetricDataPayload, CreateCalorimetricDataError, CreateCalorimetricDataErrorCode>
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    protected override CreateCalorimetricDataPayload NewPayload(
        CalorimetricData? data,
        IReadOnlyCollection<CreateCalorimetricDataError>? errors
    ) => new(data, errors);

    protected override CreateCalorimetricDataError NewError(
        CreateCalorimetricDataErrorCode code,
        string message,
        IReadOnlyList<string> path
    ) => new(code, message, path);

    public async Task<CreateCalorimetricDataPayload> CreateCalorimetricDataAsync(
        CreateCalorimetricDataInput input,
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
                CreateCalorimetricDataErrorCode.UNAUTHENTICATED,
                CreateCalorimetricDataErrorCode.UNAUTHORIZED,
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
                CreateCalorimetricDataErrorCode.ILLEGAL_CREATED_AT,
                componentByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_COMPONENT,
                institutionByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_CREATOR,
                methodByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_APPLIED_METHOD,
                dataByDatabaseAndIdAndKindDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_DATABASE,
                CreateCalorimetricDataErrorCode.UNKNOWN_DATA,
                dataFormatByIdDataLoader,
                CreateCalorimetricDataErrorCode.UNKNOWN_DATA_FORMAT,
                clock,
                cancellationToken
            )
            ).Failed(out var dataFormat, out var validateErrorPayload)
        )
        {
            return validateErrorPayload;
        }

        var calorimetricData = input.ToDomainModel(
            currentUserOrInstitution.CurrentUser?.Uuid,
            dataFormat.Extension
        );
        context.CalorimetricData.Add(calorimetricData);
        await context.SaveChangesAsync(cancellationToken);

        if ((await CreateResponseApprovalAsync(
                calorimetricData,
                CreateCalorimetricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                responseApprovalService,
                context,
                cancellationToken
            )
            ).Failed(out var createResponseApprovalErrorPayload)
        )
        {
            context.Remove(calorimetricData);
            await context.SaveChangesAsync(cancellationToken);
            return createResponseApprovalErrorPayload;
        }

        return NewPayload(calorimetricData, null);
    }
}