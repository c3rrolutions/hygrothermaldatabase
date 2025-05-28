using Microsoft.EntityFrameworkCore;

namespace Database.Data;

[Owned]
public sealed class CielabColor(
    double lStar,
    double aStar,
    double bStar
    )
{
    public double LStar { get; private set; } = lStar;
    public double AStar { get; private set; } = aStar;
    public double BStar { get; private set; } = bStar;
}