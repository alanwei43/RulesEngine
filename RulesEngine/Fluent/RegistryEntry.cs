using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace RulesEngine.Fluent
{
    public interface IFluentScopeKey<T>
    {
        string Key { get; }
    }

    internal static class RegistryKeys
    {
        public static readonly IFluentScopeKey<IRule> Rule = Key<IRule>("Rule");
        public static readonly IFluentScopeKey<LambdaExpression> SetupExpression = Key<LambdaExpression>("SetupExpression");
        public static readonly IFluentScopeKey<bool> IsCrossField = Key<bool>("IsCrossField");
        public static readonly IFluentScopeKey<Type> SourceType = Key<Type>("SourceType");
        public static readonly IFluentScopeKey<IValueResolverFactory> ValueResolverFactory = Key<IValueResolverFactory>("ValueResolverFactory");
        public static readonly IFluentScopeKey<LambdaExpression> ConditionalExpression = Key<LambdaExpression>("ConditionalExpression");
        public static readonly IFluentScopeKey<IFluentNode> FluentNodeParent = Key<IFluentNode>("FluentNodeParent");
        public static readonly IFluentScopeKey<IEngine> UsingEngine = Key<IEngine>("UsingEngine");
        public static readonly IFluentScopeKey<MessageEntry> MessageEntry = Key<MessageEntry>("MessageEntry");
        public static readonly IFluentScopeKey<Func<string>> Message = Key<Func<string>>("Message");
        public static readonly IFluentScopeKey<ICulpritResolverFactory> CulpritResolverFactory = Key<ICulpritResolverFactory>("CulpritResolverFactory");
        public static readonly IFluentScopeKey<ICulpritResolver> CulpritResolver = Key<ICulpritResolver>("CulpritResolver");
        public static readonly IFluentScopeKey<FluentScopeAction> FluentScopeAction = Key<FluentScopeAction>("FluentScopeAction");
        public static readonly IFluentScopeKey<FluentScope> IfScope = Key<FluentScope>("IfScope");

        private struct PropertyBagKey<T> : IFluentScopeKey<T>
        {
            public string Key { get; set; }
        }

        private static IFluentScopeKey<T> Key<T>(string key)
        {
            var result = new PropertyBagKey<T>() { Key = key };
            return result;
        }
    }
}
