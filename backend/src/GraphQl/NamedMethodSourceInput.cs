using Database.Data;

namespace Database.GraphQl;

public sealed record NamedMethodSourceInput(
    string Name,
    CrossDatabaseDataReferenceInput Value
)
{
    public NamedMethodSource ToDomainModel()
    {
        return new NamedMethodSource(
            Name,
            Value.ToDomainModel()
        );
    }
};