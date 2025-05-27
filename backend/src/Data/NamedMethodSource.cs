using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class NamedMethodSource(
    string name
    )
{
    public NamedMethodSource(
        string name,
        CrossDatabaseDataReference value
    )
        : this(name)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; } = name;
    public CrossDatabaseDataReference Value { get; private set; } = default!;
}