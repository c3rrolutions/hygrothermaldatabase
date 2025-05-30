using Database.Data;
using System.Collections.Generic;

namespace Database.GraphQl.DataApprovals;

public abstract class DataApprovalPayload<TApprovalError>
    : Payload
    where TApprovalError : IUserError
{
    protected DataApprovalPayload(
        DataApproval dataApproval
    )
    {
        DataApproval = dataApproval;
    }

    protected DataApprovalPayload(
        IReadOnlyCollection<TApprovalError> errors
    )
    {
        Errors = errors;
    }

    protected DataApprovalPayload(
        TApprovalError error
    )
        : this([error])
    {
    }

    protected DataApprovalPayload(
        DataApproval dataApproval,
        IReadOnlyCollection<TApprovalError> errors
    )
    {
        DataApproval = dataApproval;
        Errors = errors;
    }

    protected DataApprovalPayload(
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