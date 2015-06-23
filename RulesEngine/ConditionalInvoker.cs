using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using RulesEngine.Fluent;

namespace RulesEngine
{
    public class ConditionalInvoker<T> : IRuleInvoker
    {
        Func<T, bool> _condition;
        IEngine _innerTrue;
        IEngine _innerFalse;

        public ConditionalInvoker(Expression<Func<T, bool>> conditionalExpression, IEngine innerTrue, IEngine innerFalse)
        {
            _condition = conditionalExpression.Compile();
            _innerTrue = innerTrue;
            _innerFalse = innerFalse;
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            if (_condition.Invoke((T)value))
            {
                _innerTrue.Validate(value, report, depth);
            }
            else
            {
                _innerFalse.Validate(value, report, depth);
            }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }
    }
}
