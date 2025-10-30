using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;
using System.Diagnostics.CodeAnalysis;
using HotChocolate;
using System.Collections.Generic;
using Database.Extensions;

namespace Database.GraphQl.GeometricDataX;

public sealed record CreateGeometricDataInput(
    [GraphQLType<NonNullType<LocaleType>>] string Locale,
    Guid ComponentId,
    string? Name,
    string? Description,
    string[] Warnings,
    DateTime CreatedAt,
    Guid CreatorId,
    AppliedMethodInput AppliedMethod,
    RootGetHttpsResourceInput RootResource,
    double[] Thicknesses
)
{
    public GeometricData ToDomainModel(Guid userId)
    {
        var geometricData = new GeometricData(
            userId,
            Locale,
            ComponentId,
            Name,
            Description,
            Warnings,
            CreatorId,
            CreatedAt,
            AppliedMethod.ToDomainModel(),
            Thicknesses
        );
        geometricData.Resources.Add(RootResource.ToDomainModel());
        return geometricData;
    }
};

[SuppressMessage("Naming", "CA1707")]
public enum CreateGeometricDataErrorCode
{
    UNKNOWN,
    UNAUTHORIZED,
    UNAUTHENTICATED,
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
: DataMutationBase<GeometricData, CreateGeometricDataPayload, CreateGeometricDataError, CreateGeometricDataErrorCode>
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

    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        CommonAuthorization authorization,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        if ((await AuthorizeAsync(
                CreateGeometricDataErrorCode.UNAUTHENTICATED,
                CreateGeometricDataErrorCode.UNAUTHORIZED,
                authorization,
                cancellationToken
            )
            ).Failed(out var currentUser, out var authorizeErrorPayload)
        )
        {
            return authorizeErrorPayload;
        }

        var geometricData = input.ToDomainModel(currentUser.Uuid);
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