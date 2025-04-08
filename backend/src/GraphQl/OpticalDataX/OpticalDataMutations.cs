using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.GeometricDataX;
using Database.Services;
using HotChocolate.Types;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace Database.GraphQl.OpticalDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class OpticalDataMutations
{
    // [UseUserManager] [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateOpticalDataPayload> CreateOpticalDataAsync(
        CreateOpticalDataInput input,
        ApplicationDbContext context,
        IUserService userService,
        IResponseApprovalService responseApprovalService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(
            cancellationToken).ConfigureAwait(false);
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
        if (!OpticalDataAuthorization.IsAuthorizedToCreateOpticalDataForInstitution(
            currentUser,
            input.CreatorId,
            cancellationToken
            )
        )
            return new CreateOpticalDataPayload(
                new CreateOpticalDataError(
                    CreateOpticalDataErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to create optical data for the institution.",
                    []
                )
            );
        var opticalData = new OpticalData(
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
                        // TODO Turn `a.Value` into `JsonDocument`. It comes
                        // as nested `IReadOnlyDictionary/-List` as said on
                        // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                        // Take inspiration from
                        // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                        // and
                        // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                        // This is also needed in `GetHttpsResourceMutations`.
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
                            // TODO Turn `a.Value` into `JsonDocument`. It comes
                            // as nested `IReadOnlyDictionary/-List` as said on
                            // https://chillicream.com/docs/hotchocolate/v11/defining-a-schema/scalars/#any-type
                            // Take inspiration from
                            // https://josef.codes/custom-dictionary-string-object-jsonconverter-for-system-text-json/
                            // and
                            // https://github.com/joseftw/JOS.SystemTextJsonDictionaryStringObjectJsonConverter/blob/develop/src/JOS.SystemTextJsonDictionaryObjectModelBinder/DictionaryStringObjectJsonConverter.cs
                            // This is also needed in `GetHttpsResourceMutations`.
                            JsonDocument.Parse(@"""TODO""")
                        )
                    ).ToList(),
                    input.RootResource.AppliedConversionMethod.SourceName
                )
        );
        opticalData.Resources.Add(resource);
        context.OpticalData.Add(opticalData);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            opticalData.Approval = await responseApprovalService.CreateResponseApproval(opticalData, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Remove(opticalData);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new CreateOpticalDataPayload(
                opticalData,
                new CreateOpticalDataError(
                    CreateOpticalDataErrorCode.SIGNING_FAILED,
                    $"Signing failed with message: {ex.Message}",
                    []
                )
            );
        }
        return new CreateOpticalDataPayload(opticalData);
    }
}