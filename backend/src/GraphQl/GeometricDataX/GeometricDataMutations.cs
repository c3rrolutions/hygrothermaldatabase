using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Services;
using HotChocolate.Types;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class GeometricDataMutations
{
    // [UseUserManager]
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        IUserService userService,
        IResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken).ConfigureAwait(false);
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

        if (!GeometricDataAuthorization.IsAuthorizedToCreateGeometricDataForInstitution(
             currentUser,
             input.CreatorId,
             cancellationToken
             )
        )
            return new CreateGeometricDataPayload(
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create geometric data for the institution.",
                    []
                )
            );

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
                        JsonDocument.Parse(@"""TODO""")
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
            input.RootResource.HashValue,
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
                            JsonDocument.Parse(@"""TODO""")
                        )
                    ).ToList(),
                    input.RootResource.AppliedConversionMethod.SourceName
                )
        );
        geometricData.Resources.Add(resource);

        context.GeometricData.Add(geometricData);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            geometricData.Approval = await responseApprovalService.CreateResponseApproval(geometricData, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Remove(geometricData);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new CreateGeometricDataPayload(
                geometricData,
                new CreateGeometricDataError(
                    CreateGeometricDataErrorCode.SIGNING_FAILED,
                    $"Signing failed with message: {ex.Message}",
                    []
                )
            );
        }
        return new CreateGeometricDataPayload(geometricData);
    }
}