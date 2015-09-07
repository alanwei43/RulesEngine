using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine
{
    public interface ISetupClass
    {
    }
    public class SetupClassElseEndIf<T, R, ENDIF> : IMustPassRule<SetupClassElseEndIf<T, R, ENDIF>, T, R>, ISetupClass
    {
        Expression<Func<T, R>> _expression;
        ForClassElseEndIf<T, ENDIF> _parent;
        Engine _rulesEngine;
        IRule _lastRule;

        internal SetupClassElseEndIf(Engine rulesEngine, ForClassElseEndIf<T, ENDIF> parent, Expression<Func<T, R>> expression)
        {
            _expression = expression;
            _parent = parent;
            _rulesEngine = rulesEngine;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            _rulesEngine.RegisterRule(rule, _expression, _expression);
            _lastRule = rule;
            return this;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            _rulesEngine.RegisterRule(rule, Utilities.ReturnSelf<T>(), _expression);
            _lastRule = rule;
            return this;
        }


        public SetupClassElseEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return new SetupClassElseEndIf<T, R1, ENDIF>(_rulesEngine, _parent, expression);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return _parent.Else();
        }

        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> condition)
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


        public SetupClassElseEndIf<T, R, ENDIF> GetSelf()
        {
            return this;
        }


        public IRule LastRule
        {
            get { return _lastRule; }
        }
    }
}
