using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using Database.Authorization;
using Database.Data;
using Database.Services;
using Database.Utilities;

namespace Database.GraphQl.PhotovoltaicDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class PhotovoltaicDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreatePhotovoltaicDataPayload> CreatePhotovoltaicDataAsync(
        CreatePhotovoltaicDataInput input,
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
            return new CreatePhotovoltaicDataPayload(
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.UNAUTHENTICATED,
                    $"The user is not authenticated.",
                    []
                )
            );
        }

        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
        {
            return new CreatePhotovoltaicDataPayload(
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create photovoltaic data in this database.",
                    []
                )
            );
        }

        var photovoltaicData = new PhotovoltaicData(
            currentUser.Uuid,
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
        photovoltaicData.Resources.Add(resource);
        context.PhotovoltaicData.Add(photovoltaicData);
        await context.SaveChangesAsync(cancellationToken);

        try
        {
            photovoltaicData.Approval = await responseApprovalService.CreateResponseApproval(photovoltaicData, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            context.Remove(photovoltaicData);
            await context.SaveChangesAsync(cancellationToken);

            return new CreatePhotovoltaicDataPayload(
                photovoltaicData,
                new CreatePhotovoltaicDataError(
                    CreatePhotovoltaicDataErrorCode.CREATING_RESPONSE_APPROVAL_FAILED,
                    $"Signing failed with message: {exception.Message}",
                    []
                )
            );
        }
        return new CreatePhotovoltaicDataPayload(photovoltaicData);
    }
}