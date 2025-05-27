using Database.Data;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

public sealed class GetHttpsResourceTreeRoot(
    GetHttpsResource value
    ) : IGetHttpsResourceTreeVertex
{
    [GraphQLType<NonNullType<IdType>>] public string VertexId => GetHttpsResource.ConstructVertexId(Value.Id);

    public GetHttpsResource Value { get; } = value;
}