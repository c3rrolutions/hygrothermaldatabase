using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.ResponseApprovals;

public sealed class CreateResponseApprovalsPayload
    : Payload
{
    public IReadOnlyCollection<IData>? Data { get; }
    public IReadOnlyCollection<CreateResponseApprovalsError>? Errors { get; }

    public CreateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data
    )
    {
        Data = data;
    }

    public CreateResponseApprovalsPayload(
        CreateResponseApprovalsError error
    )
    {
        Errors = [error];
    }

    public CreateResponseApprovalsPayload(
        IReadOnlyCollection<IData> data,
        IReadOnlyCollection<CreateResponseApprovalsError> errors
    ) : this(data)
    {
        Errors = errors;
    }
}