using System;
using System.Linq;

namespace RulesEngine.Fluent
{
    public static class MessageHelper
    {
        public static M WithMessage<M, T, R>(this IMustPassRule<M, T, R> mpr, string message)
        {
            if (message == null) throw new System.ArgumentNullException("message");
            return WithMessage(mpr, () => message);
        }
        public static M WithMessage<M, T, R>(this IMustPassRule<M, T, R> mpr, Func<string> message)
        {
            if (message == null) throw new System.ArgumentNullException("message");
            var fluentNode = AsNode(mpr);

            var tmpScope = fluentNode.FluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.Message, message);
            tmpScope.Set(RegistryKeys.FluentScopeAction, new FluentScopeAction(BuilderHelper.RegisterMessage));
            return (M)fluentNode.GetSelf(tmpScope);



            
            
            ////TODO: This is not the best. Should I be assuming that the properties will have children here.
            ////This is because the Setup entry adds a scope for each new rule.
            ////Here I am interested in the scope for the rule, not the Setup one.
            //if (mpr.Properties.Children.Any())
            //{
            //    var fluentScope = mpr.Properties.Children.Last();
            //    var messageEntry = new MessageEntry(typeof(T), new EquatableExpression(mpr.Expression), fluentScope.Get(RegistryKeys.Rule, null), message);
            //    mpr.Properties.Children.Last().Set(RegistryKeys.MessageEntry, messageEntry);
            //}
            //else
            //{
            //    var messageEntry = new MessageEntry(typeof(T), new EquatableExpression(mpr.Expression), null, message);
            //    mpr.Properties.Set(RegistryKeys.MessageEntry, messageEntry);
            //}
            //return (M)mpr;
        }

        //TODO: This is repeated code (from RulesHelper, make it reuseable).
        private static IFluentNode AsNode<M, T, R>(IMustPassRule<M, T, R> mpr)
        {
            var result = mpr as IFluentNode;
            if (result == null) throw new InvalidOperationException("IMustPassRule is not a valid node.");
            return result;
        }

        private static FluentScope GetFluentScope<M, T, R>(IMustPassRule<M, T, R> mpr)
        {
            return AsNode(mpr).FluentScope;
        }

    }
}
