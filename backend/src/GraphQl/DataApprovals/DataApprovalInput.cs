using System;
using Database.GraphQl.References;

namespace Database.GraphQl.DataApprovals;

public sealed record DataApprovalInput
(
    Guid DataId,
    Guid CreatorId,
    DateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    string Response,
    Guid ApproverId,
    ReferenceInput Statement
);