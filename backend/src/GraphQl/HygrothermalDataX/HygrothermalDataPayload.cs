using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.HygrothermalDataX;

public abstract class HygrothermalDataPayload<THygrothermalDataError>
    : Payload
    where THygrothermalDataError : IUserError
{
    protected HygrothermalDataPayload(
        HygrothermalData hygrothermalData
    )
    {
        HygrothermalData = hygrothermalData;
    }

    protected HygrothermalDataPayload(
        IReadOnlyCollection<THygrothermalDataError> errors
    )
    {
        Errors = errors;
    }

    protected HygrothermalDataPayload(
        THygrothermalDataError error
    )
        : this([error])
    {
    }

    protected HygrothermalDataPayload(
        HygrothermalData hygrothermalData,
        IReadOnlyCollection<THygrothermalDataError> errors
    )
    {
        HygrothermalData = hygrothermalData;
        Errors = errors;
    }

    protected HygrothermalDataPayload(
        HygrothermalData hygrothermalData,
        THygrothermalDataError error
    )
        : this(
            hygrothermalData,
            [error]
        )
    {
    }

    public HygrothermalData? HygrothermalData { get; }
    public IReadOnlyCollection<THygrothermalDataError>? Errors { get; }
}