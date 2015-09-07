using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public class ConditionalInvoker<T> : IRuleInvoker, IRegisterInvoker
    {
        Func<T, bool> _condition;
        Engine _parent;
        Engine _innerTrue;
        Engine _innerFalse;

        public ConditionalInvoker(Expression<Func<T, bool>> conditionalExpression, Engine parent)
        {
            _condition = conditionalExpression.Compile();
            _parent = parent;
            _innerTrue = new Engine();
            _innerTrue.Parent = parent;

            _innerFalse = new Engine();
            _innerFalse.Parent = parent;
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

        public void RegisterInvoker(IRuleInvoker ruleInvoker)
        {
            _innerTrue.RegisterInvoker(ruleInvoker);
        }

        public Engine RulesEngine
        {
            get { return _parent; }
        }

        public Engine IfTrueEngine
        {
            get { return _innerTrue; }
        }
        public Engine IfFalseEngine
        {
            get { return _innerFalse; }
        }

        public Type ParameterType
        {
            get { return typeof(T); }
        }
    }
}
