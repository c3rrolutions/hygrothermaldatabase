using System.Diagnostics.CodeAnalysis;

namespace Database.Enumerations;

[SuppressMessage("Naming", "CA1707")]
public enum CoatedSide
{
    FRONT,
    BACK,
    BOTH,
    NEITHER
}