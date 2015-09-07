using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public class RuleInvoker<T, R> : IRuleInvoker
    {
        IRule<R> _rule;
        EquatableExpression _expressionToBlame;
        Func<T, R> _compiledExpression;

        public RuleInvoker(IRule<R> rule, Expression<Func<T, R>> expressionToInvoke, EquatableExpression expressionToBlame)
        {
            _rule = rule;
            _compiledExpression = expressionToInvoke.Compile();
            _expressionToBlame = expressionToBlame;
        }

        public void Invoke(object value, IValidationReport report, ValidationReportDepth depth)
        {
            //If validating an Expression that has already failed a rule, then skip.
            if (depth == ValidationReportDepth.FieldShortCircuit && report.HasError(value, _expressionToBlame))
            {
                return;
            }

            var result = _rule.Validate(_compiledExpression.Invoke((T)value));
            if (!result.IsValid)
            {
                report.AddError(new ValidationError(_rule, _expressionToBlame, result.Arguments, value));
            }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }

    }
}
