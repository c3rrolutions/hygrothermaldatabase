using System;
using System.Collections.Generic;
using System.Linq;
using Database.GraphQl.MethodAsService;
using Database.Methods;
using Database.Services.Interfaces;

namespace Database.Services;

public class MethodCalculationService : IMethodCalculationService
{
    private List<IMethod> _methods = new List<IMethod>();

    public void AddMethod(IMethod method)
    {
        _methods.Add(method);
    }

    public bool MethodExists(Guid methodId)
    {
        return _methods.Exists(x => x.Id == methodId);
    }

    public List<DataPoint> UseMethodToCalculate(Guid methodId, List<DataPoint> dataPoints)
    {
        var method = _methods.First(x => x.Id == methodId);

        return method.Calculate(dataPoints);
    }
}