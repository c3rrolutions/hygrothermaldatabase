using System;
using System.Text.Json;
using Database.Enumerations;
using Database.GraphQl.References;

namespace Database.GraphQl.DataApprovals;

public sealed record DataApprovalInput
(
    Guid DataId,
    DataKind DataKind,
    DateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    JsonElement Variables,
    string Message,
    Guid ApproverId,
    ReferenceInput Statement
);