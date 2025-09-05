using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Database.Configuration;
using HotChocolate.Types;

namespace Database.Data;

[InterfaceType("Approval")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = GraphQlConfiguration.TypeDiscriminatorPropertyName)]
[JsonDerivedType(typeof(ResponseApproval), typeDiscriminator: nameof(ResponseApproval))]
[JsonDerivedType(typeof(DataApproval), typeDiscriminator: nameof(DataApproval))]
public interface IApproval
{
    DateTime Timestamp { get; }
    string Signature { get; }
    string KeyFingerprint { get; }
    string Query { get; }
    JsonElement Variables { get; }
    string Message { get; }
    public Guid ApproverId { get; }
}