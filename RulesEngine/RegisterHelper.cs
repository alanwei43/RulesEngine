using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;

namespace RulesEngine
{
    internal static class RegisterHelper
    {
        public static void RegisterRule<T, R>(this IRegisterInvoker register, IRule<R> rule, Expression<Func<T, R>> expressionToInvoke, LambdaExpression expressionToBlame)
        {
            RuleInvoker<T, R> ruleInvoker = new RuleInvoker<T, R>(rule, expressionToInvoke, new EquatableExpression(expressionToBlame));
            register.RegisterInvoker(ruleInvoker);
        }

        public static void RegisterComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> compositionExpression)
        {
            RegisterComposition<T, R>(register, compositionExpression, register.RulesEngine);
        }

        public static void RegisterComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> compositionExpression, Engine usingEngine)
        {
            CompositionInvoker<T, R> compositionInvoker = new CompositionInvoker<T, R>(usingEngine, compositionExpression);
            register.RegisterInvoker(compositionInvoker);
        }

        public static void RegisterEnumerableComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> enumerableCompositionExpression )
                where R : IEnumerable
        {
            RegisterEnumerableComposition<T, R>(register, enumerableCompositionExpression, register.RulesEngine);
        }

        public static void RegisterEnumerableComposition<T, R>(this IRegisterInvoker register, Expression<Func<T, R>> enumerableCompositionExpression, Engine usingEngine)
            where R : IEnumerable
        {
            EnumerableCompositionInvoker<T, R> enumerableCompositionInvoker = new EnumerableCompositionInvoker<T, R>(usingEngine, enumerableCompositionExpression);
            register.RegisterInvoker(enumerableCompositionInvoker);
        }

        public static ConditionalInvoker<T> RegisterCondition<T>(this IRegisterInvoker register, Expression<Func<T, bool>> conditionalExpression)
        {
            ConditionalInvoker<T> conditionalInvoker = new ConditionalInvoker<T>(conditionalExpression, register.RulesEngine);
            register.RegisterInvoker(conditionalInvoker);
            return conditionalInvoker;
        }

        public static void RegisterInvoker(this IRegisterInvoker register, IRuleInvoker ruleInvoker)
        {
            register.RegisterInvoker(ruleInvoker);
        }

    }
}
