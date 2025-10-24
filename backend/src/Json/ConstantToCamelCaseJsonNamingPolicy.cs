using System.Text.Json;
using GraphQL.Client.Abstractions.Utilities;

namespace Database.Json;

public sealed class ConstantToCamelCaseJsonNamingPolicy
 : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToCamelCase();
}