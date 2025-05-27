using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class NamedMethodArgument(
    string name,
    JsonDocument value
    )
        : IDisposable
{
    public string Name { get; private set; } = name;
    public JsonDocument Value { get; private set; } = value;

    public void Dispose()
    {
        Value.Dispose();
    }
}