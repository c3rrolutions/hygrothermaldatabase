using System;
using Database.GraphQl.References;

namespace Database.GraphQl.DataApprovals;

public sealed record DataApprovalInput
(
    Guid DataId,
    DateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    string Response,
    Guid ApproverId,
    ReferenceInput Statement
);