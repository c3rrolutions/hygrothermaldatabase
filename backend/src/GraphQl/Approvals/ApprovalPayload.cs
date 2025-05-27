using Database.Data;
using System.Collections.Generic;

namespace Database.GraphQl.Approvals;

public class ApprovalPayload<TApprovalError>
    : Payload
    where TApprovalError : IUserError
{
    protected ApprovalPayload(
        DataApproval dataApproval
    )
    {
        DataApproval = dataApproval;
    }

    protected ApprovalPayload(
        IReadOnlyCollection<TApprovalError> errors
    )
    {
        Errors = errors;
    }

    protected ApprovalPayload(
        TApprovalError error
    )
        : this([error])
    {
    }

    protected ApprovalPayload(
        DataApproval dataApproval,
        IReadOnlyCollection<TApprovalError> errors
    )
    {
        DataApproval = dataApproval;
        Errors = errors;
    }

    protected ApprovalPayload(
        DataApproval dataApproval,
        TApprovalError error
    )
        : this(
            dataApproval,
            [error]
        )
    {
    }

    public DataApproval? DataApproval { get; }
    public IReadOnlyCollection<TApprovalError>? Errors { get; }
}