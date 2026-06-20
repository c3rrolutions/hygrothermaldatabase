using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authentication;
using Database.Authorization;
using Database.Data;
using Database.Enumerations;
using Database.Filters;
using Database.Services;
using Database.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Controllers;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "There is not file at the file path {FilePath} of the GET HTTPS resource {GetHttpsResourceId}."
    )]
    public static partial void MissingFile(
        this ILogger<GetHttpsResourcesController> logger,
        Guid getHttpsResourceId,
        string filePath
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "The GET HTTPS resource {GetHttpsResourceId} seems not to have a data set, it says to belong to data {DataId} of kind {DataKind}."
    )]
    public static partial void MissingDataSet(
        this ILogger<GetHttpsResourcesController> logger,
        Guid getHttpsResourceId,
        Guid dataId,
        DataKind dataKind
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to load the data format {DataFormatId} of the GET HTTPS resource {GetHttpsResourceId}."
    )]
    public static partial void FailedToLoadDataFormat(
        this ILogger<GetHttpsResourcesController> logger,
        Guid getHttpsResourceId,
        Guid dataFormatId
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to extract and set values from file '{FilePath}' of GET HTTPS resource {GetHttpsResourceId} of data {DataId} of kind {DataKind}."
    )]
    public static partial void FailedToExtractAndSetValuesFromFile(
        this ILogger<GetHttpsResourcesController> logger,
        string filePath,
        Guid getHttpsResourceId,
        Guid dataId,
        DataKind dataKind,
        Exception exception
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create response approval for data with ID {DataId} of kind {DataKind}."
    )]
    public static partial void FailedToCreateResponseApproval(
        this ILogger<GetHttpsResourcesController> logger,
        Guid dataId,
        DataKind dataKind,
        Exception exception
    );
}

// Inspired by https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
// and https://github.com/dotnet/AspNetCore.Docs/blob/b4599432690b8753fc2eac23d52957f47e01997a/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/
[ApiController]
public sealed class GetHttpsResourcesController(
    FileManager fileManager,
    ILogger<GetHttpsResourcesController> logger
) : Controller
{
    private bool _disposed;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!_disposed)
        {
            // Dispose of resources held by this instance.
            _disposed = true;
        }
    }

    // Disposable types implement a finalizer.
    ~GetHttpsResourcesController()
    {
        Dispose(false);
    }

    private const string GetByIdRouteName = "GetResourceById";
    private const string GetByIdAndExtensionRouteName = "GetResourceByIdAndExtension";

    public static string ConstructGetActionRouteName(GetHttpsResource getHttpsResource) =>
        getHttpsResource.FileExtension is null
        ? GetByIdRouteName
        : GetByIdAndExtensionRouteName;

    public static object CreateGetActionRouteValues(GetHttpsResource getHttpsResource) =>
        getHttpsResource.FileExtension is null
        ? new
        {
            id = getHttpsResource.Id,
        }
        : new
        {
            id = getHttpsResource.Id,
            extension = getHttpsResource.FileExtension
        };

    [HttpGet("~/api/resources/{id:guid}", Name = GetByIdRouteName)]
    [HttpGet("~/api/resources/{id:guid}.{extension}", Name = GetByIdAndExtensionRouteName)]
    [Authorize(AuthenticationSchemes = AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme)]
    [AllowAnonymous]
    [EndpointDescription("Get an HTTP resource in the media type of its data format.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        [FromServices] IDbContextFactory<ApplicationDbContext> databaseContextFactory,
        [FromServices] AccessPolicyService accessPolicyService,
        [FromServices] IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        CancellationToken cancellationToken,
        [FromRoute] string? extension = null
    )
    {
        using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var getHttpsResource = await databaseContext.GetHttpsResourcesWithData.AsQueryable()
            .Where(_ => _.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        if (getHttpsResource is null)
        {
            return Problem(
                title: "Resource Not Found",
                detail: $"There is no GET HTTPS resource with ID '{id:D}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (extension is not null && extension != getHttpsResource.FileExtension)
        {
            return Problem(
                title: "Extension Not Found",
                detail: $"There is no GET HTTPS resource with ID '{id:D}' and the extension '{extension}'. The resource's extension is '{getHttpsResource.FileExtension}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (!getHttpsResource.DoesFileExist())
        {
            logger.MissingFile(getHttpsResource.Id, getHttpsResource.FilePath);
            return Problem(
                title: "Content Not Found",
                detail: $"The GET HTTPS resource with ID '{id:D}' and the extension '{getHttpsResource.FileExtension}' does not have content.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            logger.MissingDataSet(getHttpsResource.Id, getHttpsResource.DataId, getHttpsResource.DataKind);
            return Problem(
                title: "Data Set Not Found",
                detail: $"There is no data set associated with the GET HTTPS resource with ID '{id:D}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (!await accessPolicyService.Apply<IData, bool>(
            databaseContext => databaseContext.Data(getHttpsResource.Data.Kind).AsNoTracking()
                .Where(_ => _.Id == getHttpsResource.DataId),
            async policedData =>
            {
                var node = await policedData.SingleOrDefaultAsync(cancellationToken);
                return node is null
                    ? ([], false)
                    : ([node], true);
            },
            databaseContextFactory,
            cancellationToken
        ))
        {
            return Unauthorized();
        }
        var dataFormat = await dataFormatByIdDataLoader.LoadAsync(getHttpsResource.DataFormatId);
        if (dataFormat is null)
        {
            logger.FailedToLoadDataFormat(getHttpsResource.Id, getHttpsResource.DataFormatId);
            return Problem(
                title: "Failed To Load Data Format",
                detail: $"Failed to fetch the content type of the GET HTTPS resource with ID '{id:D}'. Please try again in a few minutes.",
                statusCode: StatusCodes.Status400BadRequest,
                instance: HttpContext.Request.Path
            );
        }
        return PhysicalFile(
            physicalPath: getHttpsResource.AbsoluteFilePath,
            contentType: dataFormat.MediaType,
            fileDownloadName: getHttpsResource.FileName,
            enableRangeProcessing: true
        );
    }

    private enum MyContentType
    {
        MULTIPART,
        PIPE,
        OTHER
    }

    public const string UploadRouteName = "UploadResource";

    public static object CreateUploadActionRouteValues(GetHttpsResource getHttpsResource) =>
        new
        {
            id = getHttpsResource.Id,
        };

    [HttpPost("~/api/resources/{id:guid}", Name = UploadRouteName)]
    [EndpointDescription("Upload file for a GET HTTP resource as 'multipart/form-data' or 'text/plain'. The file must conform to the resource's data format.")]
    [DisableFormValueModelBinding]
    [DisableRequestSizeLimit]
    [Authorize(AuthenticationSchemes = AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme)]
    [Consumes(MediaTypeNames.Multipart.FormData, MediaTypeNames.Text.Plain)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Upload(
        [FromRoute] Guid id,
        [FromServices] ApplicationDbContext context,
        [FromServices] ResponseApprovalService responseApprovalService,
        [FromServices] CommonAuthorization authorization,
        CancellationToken cancellationToken
    )
    {
        if (!await authorization.IsDatabaseOperator(cancellationToken))
        {
            return Unauthorized();
        }
        var getHttpsResource = await context.GetHttpsResourcesWithData.AsQueryable()
            .Where(_ => _.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        if (getHttpsResource is null)
        {
            return Problem(
                title: "Resource Not Found",
                detail: $"There is no GET HTTPS resource with ID '{id:D}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            logger.MissingDataSet(getHttpsResource.Id, getHttpsResource.DataId, getHttpsResource.DataKind);
            return Problem(
                title: "Data Set Not Found",
                detail: $"There is no data set associated with the GET HTTPS resource with ID '{id:D}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        Directory.CreateDirectory(GetHttpsResource.FilesDirectoryPath);
        var contentType =
            MultipartRequestHelper.IsMultipartContentType(Request.ContentType) ? MyContentType.MULTIPART
            : Request.HasFormContentType ? MyContentType.PIPE
            : MyContentType.OTHER;
        switch (contentType)
        {
            case MyContentType.MULTIPART:
                var boundary = MultipartRequestHelper.GetBoundary(Request.ContentType);
                if (string.IsNullOrWhiteSpace(boundary))
                {
                    return Problem(
                        title: "Missing Boundary",
                        detail: "Missing boundary in multipart form data.",
                        statusCode: StatusCodes.Status400BadRequest,
                        instance: HttpContext.Request.Path
                    );
                }
                await fileManager.SaveViaMultipartReaderAsync(
                    getHttpsResource.FilePath,
                    boundary,
                    Request.Body,
                    cancellationToken
                );
                break;
            case MyContentType.PIPE:
                await fileManager.SaveViaPipeReaderAsync(
                    getHttpsResource.FilePath,
                    Request.BodyReader,
                    cancellationToken
                );
                break;
            default:
                return Problem(
                    title: "Unsupported Content Type",
                    detail: "The request does neither contain valid multipart form data nor a valid form.",
                    statusCode: StatusCodes.Status400BadRequest,
                    instance: HttpContext.Request.Path
                );
        }
        List<string> errors = [];
        if (getHttpsResource.IsRoot())
        {
            try
            {
                await getHttpsResource.Data.ExtractAndSetValuesFromFile(
                    getHttpsResource.FilePath,
                    getHttpsResource.DataFormatId
                );
            }
            catch (Exception exception)
            {
                logger.FailedToExtractAndSetValuesFromFile(getHttpsResource.FilePath, getHttpsResource.Id, getHttpsResource.Data.Id, getHttpsResource.Data.Kind, exception);
                errors.Add($"Failed to extract values for data {getHttpsResource.Data.Id:D} of kind {getHttpsResource.Data.Kind} from file: '{exception.Message}'");
            }
        }
        await getHttpsResource.RecomputeHashValue(cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        try
        {
            getHttpsResource.Data.Approval = await responseApprovalService.CreateResponseApproval(getHttpsResource.Data, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.FailedToCreateResponseApproval(getHttpsResource.Data.Id, getHttpsResource.Data.Kind, exception);
            errors.Add($"Failed to create response approval for data {getHttpsResource.Data.Id:D} of kind {getHttpsResource.Data.Kind}: '{exception.Message}'");
        }
        if (errors.Count > 0)
        {
            return Problem(
                title: "Post-Processing Failed",
                detail: $"The file was uploaded and if a previous file existed it was replaced. However, the following post-processing step(s) failed: {string.Join("; ", errors)}",
                statusCode: StatusCodes.Status400BadRequest,
                instance: HttpContext.Request.Path
            );
        }
        return CreatedAtRoute(
            ConstructGetActionRouteName(getHttpsResource),
            CreateGetActionRouteValues(getHttpsResource),
            null
        );
    }
}