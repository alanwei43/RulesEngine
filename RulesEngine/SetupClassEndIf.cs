using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{

    public class SetupClassEndIf<T, R, ENDIF> : IMustPassRule<SetupClassEndIf<T, R, ENDIF>, T, R>
    {
        Expression<Func<T, R>> _expression;
        ForClassEndIf<T, ENDIF> _parent;
        Engine _rulesEngine;
        IRule _lastRule;

        internal SetupClassEndIf(Engine rulesEngine, ForClassEndIf<T, ENDIF> parent, Expression<Func<T, R>> expression)
        {
            _expression = expression;
            _parent = parent;
            _rulesEngine = rulesEngine;
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            _rulesEngine.RegisterRule(rule, _expression, _expression);
            _lastRule = rule;
            return this;
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            _rulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), _expression);
            _lastRule = rule; 
            return this;
        }


        public SetupClassEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClassEndIf<T, R1, ENDIF>(_rulesEngine, _parent, expression);
        }

        public ForClassElseEndIf<T, ForClassEndIf<T, ENDIF>> If(Expression<Func<T, bool>> condition)
        {
            return _parent.If(condition);
        }


        public ENDIF EndIf()
        {
            return _parent.EndIf();
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


        public SetupClassEndIf<T, R, ENDIF> GetSelf()
        {
            return this;
        }


        public IRule LastRule
        {
            get { return _lastRule; }
        }
    }
}
