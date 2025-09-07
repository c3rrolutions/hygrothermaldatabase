using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;

namespace Database.Methods;

public interface IMethod
{
    public string Name { get; }
    public Guid Id { get; }
    public enum StandardType { en410Visible, en410Solar, iso9050Solar }
    public List<DataPoint> Calculate(IReadOnlyList<DataPoint> dataPoints);
}