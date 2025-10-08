using System;

namespace Database.GraphQl.DataApprovals;

public sealed record RemoveDataApprovalInput
(
    Guid DataId,
    string Signature
);