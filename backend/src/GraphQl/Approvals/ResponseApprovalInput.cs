using System;

namespace Database.GraphQl.Approvals;

public sealed record ResponseApprovalInput(
    DateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    string Response,
    Guid ApproverId
);