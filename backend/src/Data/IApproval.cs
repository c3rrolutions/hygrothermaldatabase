using System;
using System.Text.Json;
using HotChocolate.Types;

namespace Database.Data;

[InterfaceType("Approval")]
public interface IApproval
{
    DateTime Timestamp { get; }
    string Signature { get; }
    string KeyFingerprint { get; }
    string Query { get; }
    JsonElement Variables { get; }
    string Message { get; }
}