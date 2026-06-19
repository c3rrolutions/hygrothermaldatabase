using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.GraphQl;
using Database.GraphQl.Scalars;
using HotChocolate;
using HotChocolate.Types;
using NodaTime;

namespace Database.Data;

[InterfaceType("Approval")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = GraphQlConstants.TypeDiscriminatorPropertyName)]
[JsonDerivedType(typeof(ResponseApproval), typeDiscriminator: nameof(ResponseApproval))]
[JsonDerivedType(typeof(DataApproval), typeDiscriminator: nameof(DataApproval))]
public interface IApproval
{
    OffsetDateTime Timestamp { get; }
    string Signature { get; }
    string KeyFingerprint { get; }

    [GraphQLType<NonNullType<GraphQlQueryType>>] string Query { get; }

    JsonElement Variables { get; }
    string Message { get; }
    public Guid ApproverId { get; }
}