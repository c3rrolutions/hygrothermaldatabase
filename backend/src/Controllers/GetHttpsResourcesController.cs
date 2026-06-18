using System;
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
    [EndpointDescription("Get an HTTP resource")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid id,
        [FromServices] ApplicationDbContext context,
        [FromServices] AccessPolicyService accessPolicyService,
        [FromServices] IDataFormatByIdDataLoader dataFormatByIdDataLoader,
        CancellationToken cancellationToken,
        [FromRoute] string? extension = null
    )
    {
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
            return Problem(
                title: "Content Not Found",
                detail: $"The GET HTTPS resource with ID '{id:D}' and the extension '{getHttpsResource.FileExtension}' does not have content.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            ModelState.AddModelError(
                nameof(id),
                $"There is no data set associated with the GET HTTPS resource with ID '{id:D}'."
            );
            return BadRequest(ModelState);
        }
        if (!await accessPolicyService.Apply<IData, bool>(
            context.Data(getHttpsResource.Data.Kind).AsNoTracking()
                .Where(_ => _.Id == getHttpsResource.DataId),
            async policedData =>
            {
                var node = await policedData.SingleOrDefaultAsync(cancellationToken);
                return node is null
                    ? ([], false)
                    : ([node], true);
            },
            context,
            cancellationToken
        ))
        {
            return Unauthorized();
        }
        var dataFormat = await dataFormatByIdDataLoader.LoadAsync(getHttpsResource.DataFormatId);
        if (dataFormat is null)
        {
            ModelState.AddModelError(
                nameof(id),
                $"Failed to fetch the content type of the GET HTTPS resource with ID '{id:D}'. Please try again in a few minutes."
            );
            return BadRequest(ModelState);
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
    [EndpointDescription("Upload file for a GET HTTP resource")]
    [DisableFormValueModelBinding]
    [DisableRequestSizeLimit]
    [Authorize(AuthenticationSchemes = AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme)]
    [Consumes(MediaTypeNames.Multipart.FormData, MediaTypeNames.Text.Plain)]
    // [AcceptsMultipartFormFile]
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
                title: "Not Found",
                detail: $"There is no GET HTTPS resource with ID '{id:D}'.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            ModelState.AddModelError(
                nameof(id),
                $"There is no data set associated with the GET HTTPS resource with ID '{id:D}'."
            );
            return BadRequest(ModelState);
        }
        Directory.CreateDirectory(GetHttpsResource.FilesDirectoryPath);
        var contentType =
            MultipartRequestHelper.IsMultipartContentType(Request.ContentType) ? MyContentType.MULTIPART
            : Request.HasFormContentType ? MyContentType.PIPE
            : MyContentType.OTHER;
        if (contentType is MyContentType.OTHER)
        {
            return BadRequest("The request does neither contain valid multipart form data nor a valid form.");
        }
        switch (contentType)
        {
            case MyContentType.MULTIPART:
                var boundary = MultipartRequestHelper.GetBoundary(Request.ContentType);
                if (string.IsNullOrWhiteSpace(boundary))
                {
                    return BadRequest("Missing boundary in multipart form data.");
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
                return BadRequest($"Unsupported content type '{contentType}'");
        }
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
                ModelState.AddModelError(
                    "file",
                    $"Failed to extract values for data {getHttpsResource.Data.Id:D} of kind {getHttpsResource.Data.Kind} from file: {exception.Message}"
                );
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
            ModelState.AddModelError(
                "data",
                $"Failed to create response approval for data {getHttpsResource.Data.Id:D} of kind {getHttpsResource.Data.Kind}."
            );
        }
        if (ModelState.ErrorCount > 0)
        {
            ModelState.AddModelError(
                nameof(id),
                "The file was uploaded and if a previous file existed it was replaced. However, some post-processing step failed. See the other error(s) for details."
            );
            return BadRequest(ModelState);
        }
        return CreatedAtRoute(
            ConstructGetActionRouteName(getHttpsResource),
            CreateGetActionRouteValues(getHttpsResource),
            null
        );
    }

    // solely for the OpenAPI documentation
    // private sealed record FormFile(IFormFile File);

    // solely for the OpenAPI documentation
    // [AttributeUsage(AttributeTargets.Method)]
    // internal sealed class AcceptsMultipartFormFileAttribute : Attribute, IEndpointParameterMetadataProvider
    // {
    //     public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder)
    //     {
    //     }

    //     public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    //     {
    //         builder.Metadata.Add(
    //             new AcceptsMetadata(
    //                 [MediaTypeNames.Multipart.FormData],
    //                 typeof(FormFile)
    //             )
    //         );
    //     }
    // }
}