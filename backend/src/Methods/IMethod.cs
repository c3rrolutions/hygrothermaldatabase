using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public interface IMethod
{
    string Name { get; }
    Guid Id { get; }
    public enum StandardType { en410Visible, en410Solar, iso9050Solar }

    List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints);
    List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, StandardType standard);
}