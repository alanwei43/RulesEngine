using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public interface ISetupClass
    {
    }
    public class SetupClassElseEndIf<T, R, ENDIF> : IMustPassRule<SetupClassElseEndIf<T, R, ENDIF>, T, R>, ISetupClass, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal SetupClassElseEndIf(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<R> rule)
        {
            return BuilderHelper.CreateMustPassRule<R, SetupClassElseEndIf<T, R, ENDIF>>(rule, _fluentScope, false);
        }

        public SetupClassElseEndIf<T, R, ENDIF> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, SetupClassElseEndIf<T, R, ENDIF>>(rule, _fluentScope, true);
        }

        public SetupClassElseEndIf<T, R1, ENDIF> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClassElseEndIf<T, R1, ENDIF>>(this, expression);
        }

        public ForClassEndIf<T, ENDIF> Else()
        {
            return BuilderHelper.CreateElseNode<ForClassEndIf<T, ENDIF>>(_fluentScope);
        }

        public ForClassElseEndIf<T, ForClassElseEndIf<T, ENDIF>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            return BuilderHelper.CreateIf<T, ForClassElseEndIf<T, ENDIF>>(_fluentScope, conditionalExpression);
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
            return new SetupClassElseEndIf<T, R, ENDIF>(fluentScope);
        }
    }
}
