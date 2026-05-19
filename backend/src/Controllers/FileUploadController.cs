using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Database.Authentication;
using Database.Authorization;
using Database.Data;
using Database.Filters;
using Database.Services;
using Database.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        this ILogger<FileUploadController> logger,
        string? fileNameForDisplay,
        string filePath
    );

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create response approval for data with ID {DataId}.")]
    public static partial void FailedToCreateResponseApproval(
        this ILogger<FileUploadController> logger,
        Guid dataId,
        Exception exception
    );
}

// Inspired by https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
// and https://github.com/dotnet/AspNetCore.Docs/blob/b4599432690b8753fc2eac23d52957f47e01997a/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/
[ApiController]
public sealed class FileUploadController(
    ILogger<FileUploadController> logger
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
    ~FileUploadController()
    {
        Dispose(false);
    }

    // The following upload methods:
    //
    // 1. Disable the form value model binding to take control of handling
    //    potentially large files.
    //
    // 2. Typically, antiforgery tokens are sent in request body. Since we
    //    don't want to read the request body early, the tokens are sent via
    //    headers. The antiforgery token filter first looks for tokens in the
    //    request header and then falls back to reading the body.

    [HttpPost("~/api/upload-file")]
    [EndpointName("UploadFile")]
    [EndpointDescription("Upload file for GET HTTP resource")]
    [DisableFormValueModelBinding]
    // TODO Add this `[RequireAntiforgeryToken]` once we know where to set the generation token cookie!
    [Authorize(AuthenticationSchemes = AuthenticationConstants.CookieAndBearerTokenAuthenticationScheme)]
    [AllowAnonymous]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UploadFile(
        [FromQuery] Guid getHttpsResourceUuid,
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
                "File",
                "The request content is not multipart."
            );
            return BadRequest(ModelState);
        }
        var getHttpsResource = await context.GetHttpsResourcesWithData.AsQueryable()
                .Where(_ => _.Id == getHttpsResourceUuid)
                .SingleOrDefaultAsync(cancellationToken);
        if (getHttpsResource is null)
        {
            ModelState.AddModelError(
                "GetHttpsResourceUuid",
                $"There is no GET HTTPS resource with UUID {getHttpsResourceUuid:D}."
            );
            return BadRequest(ModelState);
        }
        if (getHttpsResource.Data is null)
        {
            ModelState.AddModelError(
                "GetHttpsResourceUuid",
                $"There is no data set associated with the GET HTTPS resource with UUID {getHttpsResourceUuid:D}."
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
                    ModelState.AddModelError("File", "There is no content-disposition header.");
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
        return Created(nameof(FileUploadController), null);
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
}