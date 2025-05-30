using System;

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
);