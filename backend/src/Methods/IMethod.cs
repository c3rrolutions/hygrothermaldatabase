using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public interface IMethod
{
    string Name { get; }
    Guid Id { get; }

    List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints);
    List<DataPoint> Calculate(IReadOnlyList<DataPoint> spectralDataPoints, IReadOnlyList<DataPoint> weightingDataPoints, string standard);
}