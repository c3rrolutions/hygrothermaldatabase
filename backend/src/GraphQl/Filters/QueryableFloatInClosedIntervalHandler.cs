using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Utilities;

namespace Database.GraphQl.Filters;

public sealed class QueryableFloatInClosedIntervalHandler
: QueryableComparableOperationHandler
{
    public QueryableFloatInClosedIntervalHandler(
        ITypeConverter typeConverter,
        InputParser inputParser)
        : base(typeConverter, inputParser)
    {
        // CanBeNull = false;
    }

    // This is used to match the handler to all `inClosedInterval` fields
    protected override int Operation => AdditionalFilterOperations.InClosedInterval;

    public override Expression HandleOperation(
        QueryableFilterContext context,
        IFilterOperationField field,
        IValueNode value,
        object? parsedValue
    )
    {
        // The context's instance is the expression path to the property
        // e.g. ~> y.gValue
        var propertyPath = context.GetInstance();
        // The parsed value is what was specified in the query
        // e.g. ~> { lowerBound: 0.1, upperBound: 0.8 }
        var typedValue = (ClosedIntervalInput?)parsedValue
            ?? throw new InvalidOperationException();
        // Creates and returns the operation
        // e.g. ~> y.gValue >= 0.1 && y.gValue <= 0.8
        return Expression.AndAlso(
            FilterExpressionBuilder.GreaterThanOrEqual(propertyPath, typedValue.LowerBound),
            FilterExpressionBuilder.LowerThanOrEqual(propertyPath, typedValue.UpperBound)
        );
    }
}