using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Data;
using Database.Services;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class GeometricDataMutations
{
    // [UseUserManager]
    //[Authorize(Policy = Configuration.AuthConfiguration.WriteApiScope)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IUserService userService,
        ISigningService signingService,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        //var currentUser = await userService.GetCurrentUser(
        //    httpContextAccessor,
        //    cancellationToken).ConfigureAwait(false);
        //if (currentUser == null)
        //{
        //    return new CreateGeometricDataPayload(
        //        new CreateGeometricDataError(
        //            CreateGeometricDataErrorCode.UNAUTHENTICATED,
        //            $"The user is not authenticated.",
        //            []
        //        )
        //    );
        //}

        //if (!GeometricDataAuthorization.IsAuthorizedToCreateGeometricDataForInstitution(
        //     currentUser,
        //     input.CreatorId,
        //     appSettings,
        //     httpClientFactory,
        //     httpContextAccessor,
        //     cancellationToken
        //     )
        //)
        //    return new CreateGeometricDataPayload(
        //        new CreateGeometricDataError(
        //            CreateGeometricDataErrorCode.UNAUTHORIZED,
        //            $"The current user is not authorized to create geometric data for the institution.",
        //            []
        //        )
        //    );

        var geometricData = new GeometricData(
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
        var (success, signature) = await signingService.SignData(JsonSerializer.Serialize(geometricData));
        if (success)
        {
            geometricData.Approval = new ResponseApproval(DateTime.Now, signature, signingService.GetFingerprint(), "", "");
        }
        context.GeometricData.Add(geometricData);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new CreateGeometricDataPayload(geometricData);
    }
}