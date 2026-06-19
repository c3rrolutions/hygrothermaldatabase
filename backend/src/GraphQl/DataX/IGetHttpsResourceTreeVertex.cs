using Database.Data;
using HotChocolate;
using HotChocolate.Types;

namespace Database.GraphQl.DataX;

[InterfaceType("GetHttpsResourceTreeVertex")]
public interface IGetHttpsResourceTreeVertex
{
    [GraphQLType<NonNullType<IdType>>] string VertexId { get; }

    GetHttpsResource Value { get; }
}