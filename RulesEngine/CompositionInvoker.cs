using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{

    public class CompositionInvoker<T, R> : IRuleInvoker
    {
        Func<T, R> _compiledExpression;
        EquatableExpression _expression;
        IEngine _rulesEngine;

        public CompositionInvoker(IEngine rulesEngine, Expression<Func<T, R>> compositionExpression)
        {
            _rulesEngine = rulesEngine;
            _compiledExpression = compositionExpression.Compile();
            _expression = new EquatableExpression(compositionExpression);
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(value, _expression))
            {
                return;
            }

            R objToValidate = _compiledExpression.Invoke((T)value);
            if (objToValidate != null)
            {
                _rulesEngine.Validate(objToValidate, report, depth);
            }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }
    }
}
