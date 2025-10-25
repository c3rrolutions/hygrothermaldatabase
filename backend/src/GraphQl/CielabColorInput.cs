using Database.Data;
using Database.Enumerations;

namespace Database.GraphQl;

public sealed record CielabColorInput(
    double LStar,
    double AStar,
    double BStar,
    CalorimetricObserver? Observer,
    Illuminant? Illuminant
)
{
    public CielabColor ToDomainModel()
    {
        return new CielabColor(
            LStar,
            AStar,
            BStar,
            Observer,
            Illuminant
        );
    }
};