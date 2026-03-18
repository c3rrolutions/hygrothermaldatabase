using System.Linq;
using GraphQL;
using Microsoft.Extensions.Logging;

namespace Database.Logging;

public static class GraphQlErrorsTagProvider
{
    public static void RecordTags(ITagCollector collector, GraphQLError[] errors)
    {
        foreach (var (i, error) in errors.Index())
        {
            collector.Add($"[{i}].{nameof(error.Message)}", error.Message);
            if (error.Path is not null)
            {
                collector.Add($"[{i}].{nameof(error.Path)}", string.Join(".", error.Path));
            }
            if (error.Locations is not null)
            {
                foreach (var (j, location) in error.Locations.Index())
                {
                    collector.Add($"[{i}].{nameof(error.Locations)}.[{j}]", $"{location.Line}:{location.Column}");
                }
            }
            if (error.Extensions is not null)
            {
                foreach (var (j, extension) in error.Extensions.Index())
                {
                    collector.Add($"[{i}].{nameof(error.Extensions)}.[{j}].Key", extension.Key);
                    collector.Add($"[{i}].{nameof(error.Extensions)}.[{j}].Value", extension.Value);
                }
            }
        }
    }
}
