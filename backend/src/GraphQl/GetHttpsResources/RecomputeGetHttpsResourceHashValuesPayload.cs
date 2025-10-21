using System.Collections.Generic;
using Database.Data;

namespace Database.GraphQl.GetHttpsResources;

public sealed class RecomputeGetHttpsResourceHashValuesPayload
    : Payload
{
    public IReadOnlyCollection<GetHttpsResource>? GetHttpsResources { get; }
    public IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError>? Errors { get; }

    public RecomputeGetHttpsResourceHashValuesPayload(
        IReadOnlyCollection<GetHttpsResource>? getHttpsResources
    )
    {
        GetHttpsResources = getHttpsResources;
    }

    public RecomputeGetHttpsResourceHashValuesPayload(
        RecomputeGetHttpsResourceHashValuesError error
    )
    {
        Errors = [error];
    }

    public RecomputeGetHttpsResourceHashValuesPayload(
        IReadOnlyCollection<GetHttpsResource> getHttpsResources,
        IReadOnlyCollection<RecomputeGetHttpsResourceHashValuesError> errors
    )
    : this(getHttpsResources)
    {
        Errors = errors;
    }
}