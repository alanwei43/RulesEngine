using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public class ForClass<T> : IMustPassRule<ForClass<T>, T, T>, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal ForClass(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClass<T, R> Setup<R>(Expression<Func<T, R>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClass<T, R>>(this, expression);
        }

        public ForClass<T> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, ForClass<T>>(rule, _fluentScope, true);
        }

        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            return BuilderHelper.CreateIf<T, ForClass<T>>(_fluentScope, conditionalExpression);
        }

        FluentScope IFluentNode.FluentScope
        {
            get { return _fluentScope; }
        }
        object IFluentNode.GetSelf(FluentScope fluentScope)
        {
            return new ForClass<T>(fluentScope);
        }

    }
}
