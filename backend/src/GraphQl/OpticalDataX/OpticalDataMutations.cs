using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class OpticalDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
        CreateOpticalDataInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            return new CreateOpticalDataPayload(
                new CreateOpticalDataError(
                    CreateOpticalDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateOpticalDataPayload(
                new CreateOpticalDataError(
                    CreateOpticalDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create optical data in this database.",
                    []
                )
            );
        }

        var opticalData = new OpticalData(
            currentUser.Uuid,
            input.Locale,
            input.ComponentId,
            input.Name,
            input.Description,
            input.Warnings,
            input.CreatorId,
            input.CreatedAt,
            input.Type,
            input.Subtype,
            input.CoatedSide,
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
            input.NearnormalHemisphericalVisibleTransmittances,
            input.NearnormalHemisphericalVisibleReflectances,
            input.NearnormalHemisphericalSolarTransmittances,
            input.NearnormalHemisphericalSolarReflectances,
            input.InfraredEmittances,
            input.ColorRenderingIndices,
            input.CielabColors.Select(c =>
                new CielabColor(
                    c.LStar,
                    c.AStar,
                    c.BStar
                )
            ).ToList()
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
        opticalData.Resources.Add(resource);
        context.OpticalData.Add(opticalData);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            opticalData.Approval = await responseApprovalService.CreateResponseApproval(opticalData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(opticalData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateOpticalDataPayload(
                opticalData,
                new CreateOpticalDataError(
                    CreateOpticalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Creating response approval failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreateOpticalDataPayload(opticalData);
    }
}