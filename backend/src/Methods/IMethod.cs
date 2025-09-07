using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public interface IMethod
{
    public string Name { get; }
    public Guid Id { get; }
    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints);
}