using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Extensions;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Database.GraphQl.GeometricDataX;

[ExtendObjectType(nameof(Mutation))]
public sealed class GeometricDataMutations
{
    // [UseUserManager]
    // [Authorize(Policy = Configuration.AuthConfiguration.WritePolicy)]
    public async Task<CreateGeometricDataPayload> CreateGeometricDataAsync(
        CreateGeometricDataInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        if (!await GeometricDataAuthorization.IsAuthorizedToCreateGeometricDataForInstitution(
             input.CreatorId,
             appSettings,
             httpClientFactory,
             httpContextAccessor,
             cancellationToken
             ).ConfigureAwait(false)
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
            input.Approvals.Select(a =>
                new DataApproval(
                    a.Timestamp,
                    a.Signature,
                    a.KeyFingerprint,
                    a.Query,
                    a.Response,
                    a.ApproverId
                )
            ).ToList(),
            // approval: input.Approval,
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
        return new CreateGeometricDataPayload(geometricData);
    }
}