using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Database.Utilities;

public static class FileHelpers
{
    public static async Task<byte[]> ProcessFormFile<T>(
        IFormFile formFile,
        ModelStateDictionary modelState
    )
    {
        var fieldDisplayName = string.Empty;
        // Use reflection to obtain the display name for the model
        // property associated with this IFormFile. If a display
        // name isn't found, error messages simply won't show
        // a display name.
        var property =
            typeof(T).GetProperty(
                formFile.Name[(formFile.Name.IndexOf('.') + 1)..]);
        if (property is not null)
        {
            if (property.GetCustomAttribute<DisplayAttribute>() is
                DisplayAttribute displayAttribute)
            {
                fieldDisplayName = $"{displayAttribute.Name} ";
            }
        }
        // Don't trust the file name sent by the client. To display
        // the file name, HTML-encode the value.
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(
            formFile.FileName);
        // Check the file length. This check doesn't catch files that only have
        // a BOM as their content.
        if (formFile.Length == 0)
        {
            modelState.AddModelError(formFile.Name,
                $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
            return [];
        }
        try
        {
            using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            // Check the content length in case the file's only
            // content was a BOM and the content is actually
            // empty after removing the BOM.
            if (memoryStream.Length == 0)
            {
                modelState.AddModelError(formFile.Name,
                    $"{fieldDisplayName}({trustedFileNameForDisplay}) is empty.");
            }
            else
            {
                return memoryStream.ToArray();
            }
        }
        catch (Exception exception)
        {
            modelState.AddModelError(formFile.Name,
                $"{fieldDisplayName}({trustedFileNameForDisplay}) upload failed. " +
                $"Please contact the Help Desk for support. Error: {exception.HResult}");
            // Log the exception
        }
        return [];
    }

    public static async Task<byte[]> ProcessStreamedFile(
        MultipartSection section,
        ContentDispositionHeaderValue contentDisposition,
        ModelStateDictionary modelState
    )
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await section.Body.CopyToAsync(memoryStream);
            if (memoryStream.Length == 0)
            {
                modelState.AddModelError("File", "The file is empty.");
            }
            else
            {
                return memoryStream.ToArray();
            }
        }
        catch (Exception exception)
        {
            modelState.AddModelError("File",
                "The upload failed. Please contact the Help Desk " +
                $" for support. Error: {exception.HResult}");
            // Log the exception
        }
        return [];
    }
}