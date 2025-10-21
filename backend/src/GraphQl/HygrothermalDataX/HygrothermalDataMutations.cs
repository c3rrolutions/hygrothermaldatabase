using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.HygrothermalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class HygrothermalDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateHygrothermalDataPayload> CreateHygrothermalDataAsync(
        CreateHygrothermalDataInput input,
        ApplicationDbContext context,
        UserService userService,
        ResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken);
        if (currentUser is null)
        {
            return new CreateHygrothermalDataPayload(
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreateHygrothermalDataPayload(
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create hygrothermal data in this database.",
                    []
                )
            );
        }

        var hygrothermalData = new HygrothermalData(
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
            )
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
        hygrothermalData.Resources.Add(resource);
        context.HygrothermalData.Add(hygrothermalData);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            hygrothermalData.Approval = await responseApprovalService.CreateResponseApproval(hygrothermalData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(hygrothermalData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateHygrothermalDataPayload(
                hygrothermalData,
                new CreateHygrothermalDataError(
                    CreateHygrothermalDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreateHygrothermalDataPayload(hygrothermalData);
    }
}