using System;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public class SetupClassEndIf<T, R, ENDIF> : IMustPassRule<SetupClassEndIf<T, R, ENDIF>, T, R>, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal SetupClassEndIf(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            return BuilderHelper.CreateMustPassRule<R, SetupClassEndIf<T, R, ENDIF>>(rule, _fluentScope, false);
        }

        public SetupClassEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, SetupClassEndIf<T, R, ENDIF>>(rule, _fluentScope, true);
        }

        public SetupClassEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClassEndIf<T, R1, ENDIF>>(this, expression);
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
            return new SetupClassEndIf<T, R, ENDIF>(fluentScope);
        }
    }
}
