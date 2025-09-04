using System.Text.Json.Serialization;
using Database.Configuration;

namespace Database.Data;

[JsonPolymorphic(TypeDiscriminatorPropertyName = GraphQlConfiguration.TypeDiscriminatorPropertyName)]
[JsonDerivedType(typeof(Standard), typeDiscriminator: nameof(Standard))]
[JsonDerivedType(typeof(Publication), typeDiscriminator: nameof(Publication))]
public interface IReference
{
}