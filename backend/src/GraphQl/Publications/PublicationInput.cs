using System;
using Database.Data;

namespace Database.GraphQl.Publications;

public sealed record PublicationInput(
    string? Title,
    string? Abstract,
    string? Section,
    string[]? Authors,
    string? Doi,
    string? ArXiv,
    string? Urn,
    Uri? WebAddress
)
{
    public Publication ToDomainModel()
    {
        return new Publication(
            Title,
            Abstract,
            Section,
            Authors,
            Doi,
            ArXiv,
            Urn,
            WebAddress
        );
    }
};