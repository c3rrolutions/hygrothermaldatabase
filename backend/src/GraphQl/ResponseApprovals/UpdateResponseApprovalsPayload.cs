using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.ResponseApprovals;

public sealed class UpdateResponseApprovalsPayload
    : Payload
{
    public IReadOnlyCollection<IData>? Data { get; }
    public IReadOnlyCollection<UpdateResponseApprovalsError>? Errors { get; }

    public UpdateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data
    )
    {
        Data = data;
    }

    public UpdateResponseApprovalsPayload(
        UpdateResponseApprovalsError error
    )
    {
        Errors = [error];
    }

    public UpdateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data,
        IReadOnlyCollection<UpdateResponseApprovalsError> errors
    ) : this(data)
    {
        Errors = errors;
    }
}