using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public class ForClass<T> : IMustPassRule<ForClass<T>, T, T>
    {
        private Engine _rulesEngine;
        private IRule _lastRule;

        internal ForClass(Engine rulesEngine)
        {
            _rulesEngine = rulesEngine;
        }
        public SetupClass<T, R> Setup<R>(Expression<Func<T, R>> expression)
        {
            return new SetupClass<T, R>(_rulesEngine, this, expression);
        }


        public ForClass<T> MustPassRule(IRule<T> rule)
        {
            Expression<Func<T, T>> expression = t => t;
            _rulesEngine.RegisterRule<T, T>(rule, expression, expression);
            _lastRule = rule;
            return this;
        }
        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            var invoker = _rulesEngine.RegisterCondition(conditionalExpression);
            return new ForClassElseEndIf<T, ForClass<T>>(invoker.IfTrueEngine, invoker, this);
        }
        public Expression<Func<T, T>> Expression
        {
            get
            {
                return Utilities.ReturnSelf<T>();
            }
        }

        /// <summary>
        /// Gets RulesEngine
        /// </summary>
        public Engine RulesEngine
        {
            get { return _rulesEngine; }
        }


        public ForClass<T> GetSelf()
        {
            return this;
        }


        public IRule LastRule
        {
            get { return _lastRule; }
        }
    }
}
