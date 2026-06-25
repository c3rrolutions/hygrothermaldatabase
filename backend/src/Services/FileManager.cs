using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Database.Services;

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Processing file '{FileName}'")]
    public static partial void ProcessingFile(
        this ILogger<FileManager> logger,
        string? fileName
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Received metadata: {Key} = {Value}")]
    public static partial void ReceivedMetadata(
        this ILogger<FileManager> logger,
        string? key,
        string value
    );

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "File upload completed (via {Type}). Total bytes read: {TotalBytesRead} bytes.")]
    public static partial void UploadedFile(
        this ILogger<FileManager> logger,
        string type,
        long totalBytesRead
    );

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Removed existing file '{FilePath}'")]
    public static partial void RemovedExistingFile(
        this ILogger<FileManager> logger,
        string filePath
    );
}

// Inspired by https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/mvc/models/file-uploads/samples/9.x/FileManagerSample/Services/FileManagerService.cs
public sealed class FileManager(
    ILogger<FileManager> logger
)
{
    private const int BufferSize = 16 * 1024 * 1024; // 16 MB buffer size

    public async Task SaveViaMultipartReaderAsync(
        string targetFilePath,
        string boundary,
        Stream contentStream,
        CancellationToken cancellationToken
    )
    {
        DeleteFileIfExisting(targetFilePath);
        using var targetFileStream = new FileStream(
            path: targetFilePath,
            mode: FileMode.OpenOrCreate,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: BufferSize,
            useAsync: true
        );
        var reader = new MultipartReader(boundary, contentStream);
        MultipartSection? section;
        long totalBytesRead = 0;
        // Process each section in the multipart body
        while ((section = await reader.ReadNextSectionAsync(cancellationToken)) is not null)
        {
            // Check if the section is a file
            var contentDisposition = section.GetContentDispositionHeader();
            if (contentDisposition is not null && contentDisposition.IsFileDisposition())
            {
                logger.ProcessingFile(contentDisposition.FileName.Value);
                // Write the file content to the target file
                await section.Body.CopyToAsync(targetFileStream, cancellationToken);
                totalBytesRead += section.Body.Length;
            }
            else if (contentDisposition is not null && contentDisposition.IsFormDisposition())
            {
                // Handle metadata (form fields)
                using var streamReader = new StreamReader(section.Body);
                var value = await streamReader.ReadToEndAsync(cancellationToken);
                logger.ReceivedMetadata(contentDisposition.Name.Value, value);
            }
        }
        logger.UploadedFile(nameof(MultipartReader), totalBytesRead);
    }

    public async Task SaveViaPipeReaderAsync(
        string targetFilePath,
        PipeReader contentReader,
        CancellationToken cancellationToken
    )
    {
        DeleteFileIfExisting(targetFilePath);
        long totalBytesRead = 0;
        using var targetFileStream = new FileStream(
            path: targetFilePath,
            mode: FileMode.OpenOrCreate,
            access: FileAccess.Write,
            share: FileShare.None,
            bufferSize: BufferSize,
            useAsync: true
        );
        while (true)
        {
            var readResult = await contentReader.ReadAsync(cancellationToken);
            var buffer = readResult.Buffer;
            foreach (var memory in buffer)
            {
                await targetFileStream.WriteAsync(memory, cancellationToken);
                totalBytesRead += memory.Length;
            }
            contentReader.AdvanceTo(buffer.End);
            if (readResult.IsCompleted)
            {
                break;
            }
        }
        logger.UploadedFile(nameof(PipeReader), totalBytesRead);
    }

    private void DeleteFileIfExisting(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            logger.RemovedExistingFile(filePath);
        }
    }
}