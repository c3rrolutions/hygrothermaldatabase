using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class ResponseApproval(
    DateTime timestamp,
    string signature,
    string keyFingerprint,
    string query,
    JsonElement variables,
    string message,
    Guid approverId
    )
    : IApproval
{
    public DateTime Timestamp { get; private set; } = timestamp;
    public string Signature { get; private set; } = signature;
    public string KeyFingerprint { get; private set; } = keyFingerprint;
    public string Query { get; private set; } = query;
    public JsonElement Variables { get; private set; } = variables;
    public string Message { get; private set; } = message;
    public Guid ApproverId { get; private set; } = approverId;
}