using System;
using Database.Data;
using Database.Enumerations;
using Database.GraphQl.Numerations;

namespace Database.GraphQl.Standards;

public sealed record StandardInput(
    string? Title,
    string? Abstract,
    string? Section,
    int? Year,
    CreateNumerationInput Numeration,
    Standardizer[] Standardizers,
    Uri? Locator
)
{
    public Standard ToDomainModel()
    {
        return new Standard(
            Title,
            Abstract,
            Section,
            Year,
            Standardizers,
            Locator
        )
        {
            Numeration = new Numeration(
                Numeration.Prefix,
                Numeration.MainNumber,
                Numeration.Suffix
            )
        };
    }
};