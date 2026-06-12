using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Database.ApiRequests;
using Database.Authentication;
using Database.Authorization;
using Database.Data;
using Database.Filters;
using Database.Services;
using Database.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Database.Controllers;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Uploaded file '{FileNameForDisplay}' to '{FilePath}'.")]
    public static partial void SavedUploadedFile(
        this ILogger<GetHttpsResourcesController> logger,
        string? fileNameForDisplay,
        string filePath
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create response approval for data with ID {DataId}.")]
    public static partial void FailedToCreateResponseApproval(
        this ILogger<GetHttpsResourcesController> logger,
        Guid dataId,
        Exception exception
    );
}

// Inspired by https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
// and https://github.com/dotnet/AspNetCore.Docs/blob/b4599432690b8753fc2eac23d52957f47e01997a/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/
[ApiController]
public sealed class GetHttpsResourcesController(
    ILogger<GetHttpsResourcesController> logger
) : Controller
{
    // Get the default form options so that we can use them to set the default
    // limits for request body data.
    private static readonly FormOptions s_defaultFormOptions = new();

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
                detail: $"There is no GET HTTPS resource with ID `{id:D}`.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (extension is not null && extension != getHttpsResource.FileExtension)
        {
            return Problem(
                title: "Extension Not Found",
                detail: $"There is no GET HTTPS resource with ID `{id:D}` and the extension `{extension}`. The resource's extension is `{getHttpsResource.FileExtension}`.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (!getHttpsResource.DoesFileExist())
        {
            return Problem(
                title: "Content Not Found",
                detail: $"The GET HTTPS resource with ID `{id:D}` and the extension `{getHttpsResource.FileExtension}` does not have content.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            ModelState.AddModelError(
                nameof(id),
                $"There is no data set associated with the GET HTTPS resource with ID `{id:D}`."
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
                $"Failed to fetch the content type of the GET HTTPS resource with ID `{id:D}`. Please try again in a few minutes."
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
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [AcceptsMultipartFormFile]
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
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            ModelState.AddModelError(
                "file",
                "The request content is not multipart."
            );
            return BadRequest(ModelState);
        }
        var getHttpsResource = await context.GetHttpsResourcesWithData.AsQueryable()
            .Where(_ => _.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        if (getHttpsResource is null)
        {
            return Problem(
                title: "Not Found",
                detail: $"There is no GET HTTPS resource with ID `{id:D}`.",
                statusCode: StatusCodes.Status404NotFound,
                instance: HttpContext.Request.Path
            );
        }
        if (getHttpsResource.Data is null)
        {
            ModelState.AddModelError(
                nameof(id),
                $"There is no data set associated with the GET HTTPS resource with ID `{id:D}`."
            );
            return BadRequest(ModelState);
        }
        Directory.CreateDirectory(GetHttpsResource.FilesDirectoryPath);
        var boundary = MultipartRequestHelper.GetBoundary(
            MediaTypeHeaderValue.Parse(Request.ContentType),
            s_defaultFormOptions.MultipartBoundaryLengthLimit);
        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        var section = await reader.ReadNextSectionAsync(cancellationToken);
        while (section is not null)
        {
            var hasContentDispositionHeader =
                ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition, out var contentDisposition);
            if (hasContentDispositionHeader)
            {
                // This check assumes that there's a file
                // present without form data. If form data
                // is present, this method immediately fails
                // and returns the model error.
                if (!MultipartRequestHelper.HasFileContentDisposition(
                        contentDisposition ?? throw new ArgumentException("Impossible (because `hasContentDispositionHeader` is `true`)")
                    )
                )
                {
                    ModelState.AddModelError("file", "There is no content-disposition header.");
                    return BadRequest(ModelState);
                }
                var streamedFileContent = await FileHelpers.ProcessStreamedFile(
                    section,
                    contentDisposition,
                    ModelState
                );
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using var targetStream = System.IO.File.Create(getHttpsResource.FilePath);
                await targetStream.WriteAsync(streamedFileContent, cancellationToken);
                // Don't trust the file name sent by the client. To display the
                // file name, HTML-encode the value.
#pragma warning disable CA1873 // Evaluation of this argument may be expensive and unnecessary if logging is disabled (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1873)
                logger.SavedUploadedFile(
                    WebUtility.HtmlEncode(contentDisposition.FileName.Value),
                    getHttpsResource.FilePath
                );
#pragma warning restore CA1873
                break;
            }
            // Drain any remaining section body that hasn't been consumed and
            // read the headers for the next section.
            section = await reader.ReadNextSectionAsync(cancellationToken);
        }
        if (getHttpsResource.IsRoot())
        {
            await getHttpsResource.Data.ExtractAndSetValuesFromFile(
                getHttpsResource.FilePath,
                getHttpsResource.DataFormatId
            );
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
            // TODO How can this be reported to the user?
            logger.FailedToCreateResponseApproval(getHttpsResource.Data.Id, exception);
        }
        return CreatedAtRoute(
            ConstructGetActionRouteName(getHttpsResource),
            CreateGetActionRouteValues(getHttpsResource),
            null
        );
    }

    private static Encoding GetEncoding(MultipartSection section)
    {
        var hasMediaTypeHeader =
            MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

        // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in most
        // cases.
#pragma warning disable SYSLIB0001
        if (!hasMediaTypeHeader || Encoding.UTF7.Equals(
                mediaType?.Encoding ??
                throw new ArgumentException("Impossible (because `hasMediaTypeHeader` is `true`)!"))
           )
        {
#pragma warning restore SYSLIB0001
            return Encoding.UTF8;
        }

        return mediaType.Encoding;
    }

    // solely for the OpenAPI documentation
    private sealed record FormFile(IFormFile File);

    // solely for the OpenAPI documentation
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class AcceptsMultipartFormFileAttribute : Attribute, IEndpointParameterMetadataProvider
    {
        public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder)
        {
        }

        public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            builder.Metadata.Add(
                new AcceptsMetadata(
                    [MediaTypeNames.Multipart.FormData],
                    typeof(FormFile)
                )
            );
        }
    }
}