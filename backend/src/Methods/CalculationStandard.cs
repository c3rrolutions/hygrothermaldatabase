using System.Diagnostics.CodeAnalysis;

namespace Database.Methods;

[SuppressMessage("Naming", "CA1707")]
public enum CalculationStandard
{
    EN_410_VISIBLE,
    EN_410_SOLAR,
    ISO_9050_SOLAR
}