using System;
using System.Collections.Generic;
using System.Linq;
using Database.GraphQl.MethodAsService;
using Database.Methods;
using Database.Services;

namespace Database.Services;

public sealed class MethodCalculationService
{
    private List<IMethod> _methods = [];

    public void AddMethod(IMethod method)
    {
        _methods.Add(method);
    }

    public bool MethodExists(Guid methodId)
    {
        return _methods.Exists(x => x.Id == methodId);
    }

    public List<DataPoint> UseMethodToCalculate(Guid methodId, IReadOnlyList<DataPoint> dataPoints)
    {
        var method = _methods.First(x => x.Id == methodId);

        return method.Calculate(dataPoints);
    }
}