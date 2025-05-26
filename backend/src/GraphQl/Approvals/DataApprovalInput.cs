using System;
using Database.GraphQl.Publications;
using Database.GraphQl.Standards;

namespace Database.GraphQl.Approvals;

public sealed record DataApprovalInput(
    DateTime Timestamp,
    string Signature,
    string KeyFingerprint,
    string Query,
    string Response,
    Guid ApproverId,
    CreateStandardInput? Standard,
    CreatePublicationInput? Publication
);