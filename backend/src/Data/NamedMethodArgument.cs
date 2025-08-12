using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class NamedMethodArgument(
    string name,
    JsonElement value
    )
{
    public string Name { get; private set; } = name;
    public JsonElement Value { get; private set; } = value;
}