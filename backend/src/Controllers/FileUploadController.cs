using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Database.Authorization;
using Database.Data;
using Database.Filters;
using Database.Services;
using Database.Utilities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Database.Controllers;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Uploaded file '{FileNameForDisplay}' to '{FilePath}'.")]
    public static partial void SavedUploadedFile(
        this ILogger logger,
        string? fileNameForDisplay,
        string filePath
    );
}

// Inspired by https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#upload-large-files-with-streaming
// and https://github.com/dotnet/AspNetCore.Docs/blob/b4599432690b8753fc2eac23d52957f47e01997a/aspnetcore/mvc/models/file-uploads/samples/3.x/SampleApp/
public sealed class FileUploadController(
    ILogger<FileUploadController> logger
) : Controller
{
    private const long FileSizeLimit = 10737418240; // 10 GiB = 10 * 1024 MiB = 10 * 1024 * 1024^2 Byte = 10 * 1024 * 1048576 Byte = 10737418240 Byte

    // Get the default form options so that we can use them to set the default
    // limits for request body data.
    private static readonly FormOptions s_defaultFormOptions = new();
    private readonly ILogger<FileUploadController> _logger = logger;

    private readonly string[] _permittedExtensions = [".json", ".xml", ".txt", ".csv", ".ifc", ".rad", ".svg", ".pdf", ".png"];

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
    [DisableFormValueModelBinding]
    // TODO Add this `[ValidateAntiForgeryToken]` once we know where to set the generation token cookie!
    // TODO Where to put: [GenerateAntiforgeryTokenCookie] ?
    // TODO Are both RequestFormLimits and RequestSizeLimit needed?
    // See https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#multipart-body-length-limit
    [RequestFormLimits(MultipartBodyLengthLimit = 10737418240)] // 10 GiB
    // See https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0#kestrel-maximum-request-body-size
    [RequestSizeLimit(10737418240)] // 10 GiB
    public async Task<IActionResult> UploadFile(
        [FromQuery] Guid getHttpsResourceUuid,
        [FromServices] ApplicationDbContext context,
        [FromServices] UserService userService,
        CancellationToken cancellationToken
    )
    {
        var currentUser = await userService.GetCurrentUser(cancellationToken);
        if (currentUser is null)
        {
            ModelState.AddModelError("CurrentUser",
                $"User is not authenticated.");
            return BadRequest(ModelState);
        }
        if (!CommonAuthorization.IsCurrentUserAtLeastAssistantManagerOfDatabaseOperator(currentUser))
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
        var getHttpsResource = await context.GetHttpsResources.AsQueryable()
                .Include(e => e.CalorimetricData)
                .Include(e => e.GeometricData)
                .Include(e => e.HygrothermalData)
                .Include(e => e.OpticalData)
                .Include(e => e.PhotovoltaicData)
                .Where(e => e.Id == getHttpsResourceUuid)
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
                    ModelState,
                    _permittedExtensions,
                    FileSizeLimit
                );
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using var targetStream = System.IO.File.Create(getHttpsResource.FilePath);
                await targetStream.WriteAsync(streamedFileContent, cancellationToken);
                // Don't trust the file name sent by the client. To display the
                // file name, HTML-encode the value.
                _logger.SavedUploadedFile(
                    WebUtility.HtmlEncode(contentDisposition.FileName.Value),
                    getHttpsResource.FilePath
                );
                break;
            }
            // Drain any remaining section body that hasn't been consumed and
            // read the headers for the next section.
            section = await reader.ReadNextSectionAsync(cancellationToken);
        }
        await getHttpsResource.RecomputeHashValue(cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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