using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class DataApproval(
    OffsetDateTime timestamp,
    string signature,
    string keyFingerprint,
    string query,
    JsonElement variables,
    string message,
    Guid approverId,
    Reference statement
    )
        : IApproval
{
    // Constructor for EF Core because navigation properties cannot be set using a constructor: https://learn.microsoft.com/en-us/ef/core/modeling/constructors#binding-to-mapped-properties
    private DataApproval(
        OffsetDateTime timestamp,
        string signature,
        string keyFingerprint,
        string query,
        JsonElement variables,
        string message,
        Guid approverId
    )
    : this(
        timestamp,
        signature,
        keyFingerprint,
        query,
        variables,
        message,
        approverId,
        null! // EF Core will set this owned navigation property after construction.
        )
    {
    }

    public Guid ApproverId { get; private set; } = approverId;
    public OffsetDateTime Timestamp { get; private set; } = timestamp;
    public string Signature { get; private set; } = signature;
    public string KeyFingerprint { get; private set; } = keyFingerprint;
    public string Query { get; private set; } = query;
    public JsonElement Variables { get; private set; } = variables;
    public string Message { get; private set; } = message;
    public Reference Statement { get; private set; } = statement;
}