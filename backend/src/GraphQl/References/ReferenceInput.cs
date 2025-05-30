using Database.GraphQl.Publications;
using Database.GraphQl.Standards;

namespace Database.GraphQl.References;

public sealed record ReferenceInput(
    StandardInput? Standard,
    PublicationInput? Publication
);