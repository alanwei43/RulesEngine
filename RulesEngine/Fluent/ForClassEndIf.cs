using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using RulesEngine.Fluent;

namespace RulesEngine
{
    public class ForClassEndIf<T, ENDIF> : IMustPassRule<ForClassEndIf<T, ENDIF>, T, T>, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal ForClassEndIf(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClassEndIf<T, R, ENDIF> Setup<R>(Expression<Func<T, R>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClassEndIf<T, R, ENDIF>>(this, expression);
        }

        public ForClassEndIf<T, ENDIF> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, ForClassEndIf<T, ENDIF>>(rule, _fluentScope, true);
        }

        public ForClassElseEndIf<T, ForClassEndIf<T, ENDIF>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            return BuilderHelper.CreateIf<T, ForClassEndIf<T, ENDIF>>(_fluentScope, conditionalExpression);
        }

        public ENDIF EndIf()
        {
            return BuilderHelper.CreateEndIf<ENDIF>(_fluentScope);
        }

        FluentScope IFluentNode.FluentScope
        {
            get { return _fluentScope; }
        }

        object IFluentNode.GetSelf(FluentScope fluentScope)
        {
            return new ForClassEndIf<T, ENDIF>(fluentScope);
        }

    }
}
