using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public class SetupClass<T, R> : IMustPassRule<SetupClass<T, R>, T, R>, ISetupClass, IFluentNode
    {
        private readonly FluentScope _fluentScope;

        internal SetupClass(FluentScope fluentScope)
        {
            _fluentScope = fluentScope;
        }

        public SetupClass<T, R> MustPassRule(IRule<R> rule)
        {
            return BuilderHelper.CreateMustPassRule<R, SetupClass<T, R>>(rule, _fluentScope, false);
        }

        public SetupClass<T, R> MustPassRule(IRule<T> rule)
        {
            return BuilderHelper.CreateMustPassRule<T, SetupClass<T, R>>(rule, _fluentScope, true);
        }

        public SetupClass<T, R1> Setup<R1>(Expression<Func<T, R1>> expression)
        {
            return BuilderHelper.CreateSetupNode<SetupClass<T, R1>>(this, expression);
        }

        public ForClassElseEndIf<T, ForClass<T>> If(Expression<Func<T, bool>> conditionalExpression)
        {
            return BuilderHelper.CreateIf<T, ForClass<T>>(_fluentScope, conditionalExpression);
        }

        public SetupClass<T, R> Blame<R1>(Expression<Func<T, R1>> culpritExpression)
        {
            //TODO: Make this function happen (And the overload too).
            //Must be in the BuilderHelper.
            //HOW: Set a CulpritExpression and CulpritObjectExpression properties on the scope? So that the RegisterRule picks them up and knows what to do with the Blame?
            return BuilderHelper.SetBlame(this, culpritExpression);
        }
        public SetupClass<T, R> Blame<T1, R1>(Expression<Func<T, T1>> culpritObjectExpression, Expression<Func<T1, R1>> culpritExpression)
        {
            return BuilderHelper.SetBlame(this, culpritObjectExpression, culpritExpression);
        }

        public ForClass<T> EndSetup()
        {
            //TODO: Use the same concept as for the IfScope (see RegistryKeys). And have EndSetup on all setup nodes, or none at all!.
            var parentNode = _fluentScope.Get(RegistryKeys.FluentNodeParent);
            //Create a new child, but get values from the parent for class
            var tmpScope = _fluentScope.CreateChild(parentNode.FluentScope);
            return new ForClass<T>(tmpScope);
        }

        FluentScope IFluentNode.FluentScope
        {
            get { return _fluentScope; }
        }

        object IFluentNode.GetSelf(FluentScope fluentScope)
        {
            return new SetupClass<T, R>(fluentScope);
        }

    }
}
