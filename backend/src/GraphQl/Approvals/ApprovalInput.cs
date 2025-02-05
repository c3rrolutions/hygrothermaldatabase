using System;

namespace Database.GraphQl.Approvals;

public sealed record ApprovalInput
(
    Guid DataId,
    Guid CreatorId,
    DataApprovalInput Approval,
    ResponseApprovalInput ResponseApproval
);