using NodaTime;

namespace Database.GraphQl.Common;

public sealed record OpenEndedDateTimeRangeInput(
    OffsetDateTime? From,
    OffsetDateTime? Until
);