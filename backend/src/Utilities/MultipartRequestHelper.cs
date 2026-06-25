using System;
using System.Diagnostics.Contracts;
using Microsoft.Net.Http.Headers;

namespace Database.Utilities;

public static class MultipartRequestHelper
{
    // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
    // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
    [Pure]
    public static string? GetBoundary(string? contentType) =>
        HeaderUtilities.RemoveQuotes(
            MediaTypeHeaderValue.Parse(contentType).Boundary
        ).Value;

    [Pure]
    public static bool IsMultipartContentType(string? contentType) =>
        !string.IsNullOrEmpty(contentType)
        && contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase);
}