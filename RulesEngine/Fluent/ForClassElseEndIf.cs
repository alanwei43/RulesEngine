using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public class ForClassElseEndIf<T, ENDIF> : IMustPassRule<ForClassElseEndIf<T, ENDIF>, T, T>, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal ForClassElseEndIf(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClassElseEndIf<T, R, ENDIF> Setup<R>(Expression<Func<T, R>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClassElseEndIf<T, R, ENDIF>>(this, expression);
        }

        public ForClassElseEndIf<T, ENDIF> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, ForClassElseEndIf<T, ENDIF>>(rule, _fluentScope, true);
        }
        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            return BuilderHelper.CreateIf<T, ForClassElseEndIf<T, ENDIF>>(_fluentScope, conditionalExpression);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return BuilderHelper.CreateElseNode<ForClassEndIf<T, ENDIF>>(_fluentScope);
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
            return new ForClassElseEndIf<T, ENDIF>(fluentScope);
        }
    }
}
