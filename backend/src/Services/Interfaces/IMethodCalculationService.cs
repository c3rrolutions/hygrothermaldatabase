using System;
using System.Collections.Generic;
using Database.GraphQl.MethodAsService;
using Database.Methods;

namespace Database.Services.Interfaces;

public interface IMethodCalculationService
{
    void AddMethod(IMethod method);

    bool MethodExists(Guid methodId);

    List<DataPoint> UseMethodToCalculate(Guid methodId, List<DataPoint> dataPoints);
}