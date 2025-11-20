using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Database.ApiRequests;

public static class GraphQlQueryHelpers
{
    public static Task<string> Construct(
        string fileName
    )
    {
        return Construct([fileName]);
    }

    /// <summary>
    /// Construct query from passed files.
    /// </summary>
    /// <param name="fileNames"> Name of files containung queries. </param>
    /// <returns> Query from all files. </returns>
    public static async Task<string> Construct(
        string[] fileNames
    )
    {
        return string.Join(
            Environment.NewLine,
            await Task.WhenAll(
                fileNames.Select(fileName =>
                    File.ReadAllTextAsync($"./ApiRequests/Queries/{fileName}")
                )
            )
        );
    }
}