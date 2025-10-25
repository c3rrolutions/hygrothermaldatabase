using System.Text.Json;
using Database.Data;

namespace Database.GraphQl;

public sealed record NamedMethodArgumentInput(
    string Name,
    JsonElement Value
)
{
    public NamedMethodArgument ToDomainModel()
    {
        return new NamedMethodArgument(
            Name,
            Value
        );
    }
};