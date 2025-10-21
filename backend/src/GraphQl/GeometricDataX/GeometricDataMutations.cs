using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class GeometricDataMutations
{
    // [UseUserManager]
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new CreateGeometricDataPayload(
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateGeometricDataPayload(
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create geometric data in this database.",
                    []
                )
            );
        }

        var geometricData = new GeometricData(
            input.Locale,
            input.ComponentId,
            input.Name,
            input.Description,
            input.Warnings,
            input.CreatorId,
            input.CreatedAt,
            new AppliedMethod(
                input.AppliedMethod.MethodId,
                input.AppliedMethod.Arguments
                    .Select(a => new NamedMethodArgument(
                        a.Name,
                        a.Value
                    ))
                    .ToList(),
                input.AppliedMethod.Sources
                    .Select(s => new NamedMethodSource(
                        s.Name,
                        new CrossDatabaseDataReference(
                            s.Value.DataId,
                            s.Value.DataTimestamp,
                            s.Value.DataKind,
                            s.Value.DatabaseId
                        )
                    ))
                    .ToList()
            ),
            input.Thicknesses
        );
        var resource = new GetHttpsResource(
            input.RootResource.Description,
            Sha256FileHasher.ComputeForString(""), // The correct hash value is computed when the file for this resource is being uploaded.
            input.RootResource.DataFormatId,
            null,
            input.RootResource.ArchivedFilesMetaInformation.Select(i =>
                new FileMetaInformation(
                    i.Path,
                    i.DataFormatId
                )
            ).ToList(),
            input.RootResource.AppliedConversionMethod is null
                ? null
                : new ToTreeVertexAppliedConversionMethod(
                    input.RootResource.AppliedConversionMethod.MethodId,
                    input.RootResource.AppliedConversionMethod.Arguments.Select(a =>
                        new NamedMethodArgument(
                            a.Name,
                            a.Value
                        )
                    ).ToList(),
                    input.RootResource.AppliedConversionMethod.SourceName
                )
        );
        geometricData.Resources.Add(resource);

        context.GeometricData.Add(geometricData);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            geometricData.Approval = await responseApprovalService.CreateResponseApproval(geometricData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(geometricData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateGeometricDataPayload(
                geometricData,
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }

        return new CreateGeometricDataPayload(geometricData);
    }
}