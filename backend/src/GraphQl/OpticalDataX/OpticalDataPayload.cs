using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.OpticalDataX;

public abstract record OpticalDataPayload<TOpticalDataError>(
    OpticalData? OpticalData,
    IReadOnlyCollection<TOpticalDataError>? Errors
)
where TOpticalDataError : IUserError;