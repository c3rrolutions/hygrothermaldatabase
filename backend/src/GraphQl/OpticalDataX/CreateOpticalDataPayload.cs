using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.OpticalDataX;

public sealed record CreateOpticalDataPayload(
    OpticalData? OpticalData,
    IReadOnlyCollection<CreateOpticalDataError>? Errors
)
: OpticalDataPayload<CreateOpticalDataError>(OpticalData, Errors);