using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Database.Extractors;

public abstract class JsonExtractorBase<TOutput>
{
    public async Task<TOutput> Extract(string jsonFilePath)
    {
        using var fileStream = File.OpenRead(jsonFilePath);
        using var jsonDocument = await JsonDocument.ParseAsync(fileStream);
        return Extract(jsonDocument);
    }

    public abstract TOutput Extract(JsonDocument jsonDocument);
}