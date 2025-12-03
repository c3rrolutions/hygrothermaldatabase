using NodaTime;

namespace Database.Extensions;

public static class NodaTimeExtensions
{
    extension(OffsetDateTime)
    {
        public static OffsetDateTime UtcNow()
        {
            return
                SystemClock.Instance
                .GetCurrentInstant()
                .WithOffset(Offset.Zero);
        }
    }
}