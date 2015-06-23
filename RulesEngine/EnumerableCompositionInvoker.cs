using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace RulesEngine
{
    public class EnumerableCompositionInvoker<T, R> : IRuleInvoker
        where R : IEnumerable
    {
        Func<T, R> _compiledExpression;
        EquatableExpression _enumerableCompositionExpression;
        IEngine _rulesEngine;
        public EnumerableCompositionInvoker(IEngine rulesEngine, Expression<Func<T, R>> enumerableCompositionExpression)
        {
            _rulesEngine = rulesEngine;
            _compiledExpression = enumerableCompositionExpression.Compile();
            _enumerableCompositionExpression = new EquatableExpression(enumerableCompositionExpression);
        }
        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(value, _enumerableCompositionExpression))
            {
                return;
            }

            IEnumerable enumerableToValidate = _compiledExpression.Invoke((T)value);
            if (enumerableToValidate != null)
            {
                foreach (object objToValidate in enumerableToValidate)
                {
                    _rulesEngine.Validate(objToValidate, report, depth);
                    if (report.HasErrors && (depth == ValidationReportDepth.ShortCircuit))
                    {
                        return;
                    }
                }
            }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

    }
}
