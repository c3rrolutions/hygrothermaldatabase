using System;
using Database.Data;
using Database.GraphQl.Scalars;
using HotChocolate;

namespace Database.GraphQl.Publications;

public sealed record PublicationInput(
    string? Title,
    string? Abstract,
    string? Section,
    string[]? Authors,
    [property: GraphQLType<DoiType>] string? Doi,
    [property: GraphQLType<ArXivType>] string? ArXiv,
    [property: GraphQLType<MyUriType>] string? Urn,
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