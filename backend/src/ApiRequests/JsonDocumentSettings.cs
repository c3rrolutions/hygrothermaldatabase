using System.Text.Json;

namespace Database.ApiRequests;

public static class JsonDocumentSettings
{
    public static JsonDocumentOptions Strict => new()
    {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 0
    };

    public static JsonDocumentOptions Lax => new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
        MaxDepth = 0
    };
}