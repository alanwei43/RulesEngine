using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{

    public class SetupClass<T, R> : IMustPassRule<SetupClass<T, R>, T, R>, ISetupClass
    {
        Expression<Func<T, R>> _expression;
        ForClass<T> _parent;
        Engine _rulesEngine;
        IRule _lastRule;

        internal SetupClass(Engine rulesEngine, ForClass<T> parent, Expression<Func<T, R>> expression)
        {
            _expression = expression;
            _parent = parent;
            _rulesEngine = rulesEngine;
        }

        public SetupClass<T, R> MustPassRule(IRule<R> rule)
        {
            _rulesEngine.RegisterRule(rule, _expression, _expression);
            _lastRule = rule;
            return this;
        }

        public SetupClass<T, R> MustPassRule(IRule<T> rule)
        {
            _rulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), _expression);
            _lastRule = rule;
            return this;
        }

        public SetupClass<T, R1> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClass<T, R1>(_rulesEngine, _parent, expression);
        }

        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> condition)
        {
            return _parent.If(condition);
        }


        public ForClass<T> EndSetup()
        {
            return _parent;
        }

        public Expression<Func<T, R>> Expression
        {
            get
            {
                return _expression;
            }
        }

        /// <summary>
        /// Gets RulesEngine
        /// </summary>
        public Engine RulesEngine
        {
            get { return _rulesEngine; }
        }



        public SetupClass<T, R> GetSelf()
        {
            return this;
        }


        public IRule LastRule
        {
            get { return _lastRule; }
        }
    }
}
