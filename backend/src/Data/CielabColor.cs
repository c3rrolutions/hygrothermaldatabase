using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Database.Enumerations;
using Database.Json;

namespace Database.Data;

[Owned]
public sealed class CielabColor(
    double lStar,
    double aStar,
    double bStar,
    CalorimetricObserver? observer = null,
    Illuminant? illuminant = null
    )
{
    [Range(0.0, 100.0)]
    [JsonConverter(typeof(NumberWithUncertaintyConverter))]
    public double LStar { get; private set; } = lStar;

    [JsonConverter(typeof(NumberWithUncertaintyConverter))]
    public double AStar { get; private set; } = aStar;

    [JsonConverter(typeof(NumberWithUncertaintyConverter))]
    public double BStar { get; private set; } = bStar;

    public CalorimetricObserver? Observer { get; private set; } = observer;
    public Illuminant? Illuminant { get; private set; } = illuminant;
}