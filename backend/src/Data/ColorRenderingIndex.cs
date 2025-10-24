using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Database.Enumerations;
using Database.Json;

namespace Database.Data;

[Owned]
public sealed class ColorRenderingIndex(
    double ra,
    // double raOutIn,
    CalorimetricObserver? observer,
    Illuminant? illuminant
    )
{
    [Range(0.0, 100.0)]
    [JsonConverter(typeof(NumberWithUncertaintyConverter))]
    public double Ra { get; private set; } = ra;
    public CalorimetricObserver? Observer { get; private set; } = observer;
    public Illuminant? Illuminant { get; private set; } = illuminant;
}