using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.GraphQl.Approvals;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

    public async Task<AddApprovalPayload> AddApprovalToGeometricDataAsync(
        ApprovalInput input,
        ApplicationDbContext context,
        AppSettings appSettings,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken
    )
    {
        if (!await CommonAuthorization.IsAuthorizedToAddApprovalForInstitution(
             input.CreatorId,
             appSettings,
             httpClientFactory,
             httpContextAccessor,
             cancellationToken
             ).ConfigureAwait(false)
        )
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNAUTHORIZED,
                    $"The current user is not authorized to approval for the institution.",
                    []
                )
            );

        var data = await context.GeometricData.AsQueryable().SingleOrDefaultAsync(x => x.Id == input.DataId, cancellationToken).ConfigureAwait(false);
        if (data == null)
        {
            return new AddApprovalPayload(
                new AddApprovalError(
                    AddApprovalErrorCode.UNKNOWN,
                    $"Unknown data.",
                    []
                )
            );
        }

        var approval = new DataApproval(
                DateTime.Now,
                input.Approval.Signature,
                input.Approval.KeyFingerprint,
                input.Approval.Query,
                input.Approval.Response,
                input.CreatorId); // Should be CurrentUser Id
        data.Approvals.Add(approval);
        data.Approval = new ResponseApproval(
            DateTime.Now,
            input.ResponseApproval.Signature,
            input.ResponseApproval.KeyFingerprint,
            input.ResponseApproval.Query,
            input.ResponseApproval.Response);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return new AddApprovalPayload(approval);
    }
}