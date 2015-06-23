using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using RulesEngine.Rules;
using System.Text.RegularExpressions;

namespace RulesEngine.Fluent
{
    public static class RulesHelper
    {
        #region LessThan
        public static M MustBeLessThan<M, T, R>(this IMustPassRule<M, T, R> mpr, R lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<R>(lessThan, false));
        }

        public static M MustBeLessThan<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> lessThan)
        where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<T, R>(GetExpression(mpr), lessThan, false));
        }

        public static M MustBeLessThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, R lessThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<R>(lessThan, true));
        }

        public static M MustBeLessThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> lessThan)
        where R : IComparable<R>
        {
            return mpr.MustPassRule(new LessThanRule<T, R>(GetExpression(mpr), lessThan, true));
        }
        #endregion

        #region GreaterThan
        public static M MustBeGreaterThan<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<R>(greaterThan, false));
        }

        public static M MustBeGreaterThan<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan)
        where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<T, R>(GetExpression(mpr), greaterThan, false));
        }

        public static M MustBeGreaterThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<R>(greaterThan, true));
        }

        public static M MustBeGreaterThanOrEqualTo<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan)
        where R : IComparable<R>
        {
            return mpr.MustPassRule(new GreaterThanRule<T, R>(GetExpression(mpr), greaterThan, true));
        }
        #endregion

        #region Null Rules
        public static M MustNotBeNull<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : class
        {
            return mpr.MustPassRule(new NotNullRule<R>());
        }
        public static M MustBeNull<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : class
        {
            return mpr.MustPassRule(new NullRule<R>());
        }
        public static M MustNotBeNull<M, T, R>(this IMustPassRule<M, T, Nullable<R>> mpr)
            where R: struct
        {
            return mpr.MustPassRule(new NullableNotNullRule<R>());
        }
        public static M MustBeNull<M, T, R>(this IMustPassRule<M, T, Nullable<R>> mpr)
            where R : struct
        {
            return mpr.MustPassRule(new NullableNullRule<R>());
        }
        public static M MustNotBeNullOrEmpty<M, T>(this IMustPassRule<M, T, string> mpr)
        {
            return mpr.MustPassRule(new NotNullOrEmpty());
        }
        #endregion

        #region BetweenRule
        private const BetweenRuleBoundsOption DefaultBoundOption = BetweenRuleBoundsOption.BothInclusive;

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            return mpr.MustPassRule(new BetweenRule<T, R>(GetExpression(mpr), greaterThan, lessThan, bounds));
        }

        /// <summary>
        /// Must be Between - Inclusive bounds.
        /// </summary>
        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        /// <summary>
        /// Must be Between - Inclusive bounds.
        /// </summary>
        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, Expression<Func<T, R>> lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        /// <summary>
        /// Must be Between - Inclusive bounds.
        /// </summary>
        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, R lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        /// <summary>
        /// Must be Between - Inclusive bounds.
        /// </summary>
        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, R lessThan)
            where R : IComparable<R>
        {
            BetweenRuleBoundsOption bounds = DefaultBoundOption;
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, Expression<Func<T, R>> lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> greaterThan, R lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }
        public static M MustBeBetween<M, T, R>(this IMustPassRule<M, T, R> mpr, R greaterThan, R lessThan, BetweenRuleBoundsOption bounds)
            where R : IComparable<R>
        {
            Expression<Func<T, R>> greaterThanFunc = f => greaterThan;
            Expression<Func<T, R>> lessThanFunc = f => lessThan;
            return MustBeBetween(mpr, greaterThanFunc, lessThanFunc, bounds);
        }

        #endregion

        #region OneOf
        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, params R[] values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values, comparer));
        }

        public static M MustBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEqualityComparer<R> comparer, params R[] values)
        {
            return mpr.MustPassRule(new OneOfRule<R>(values, comparer));
        }
        #endregion

        #region NotOneOf
        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, params R[] values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEnumerable<R> values, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values, comparer));
        }

        public static M MustNotBeOneOf<M, T, R>(this IMustPassRule<M, T, R> mpr, IEqualityComparer<R> comparer, params R[] values)
        {
            return mpr.MustPassRule(new NotOneOfRule<R>(values, comparer));
        }
        #endregion

        #region EqualRule
        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value)
        {
            return mpr.MustPassRule(new EqualRule<R>(value));
        }
        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new EqualRule<R>(value, comparer));
        }
        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value)
        {
            return mpr.MustPassRule(new EqualRule<T, R>(GetExpression(mpr), value));
        }
        public static M MustEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new EqualRule<T, R>(GetExpression(mpr), value, comparer));
        }
        #endregion

        #region NotEqualRule
        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value)
        {
            return mpr.MustPassRule(new NotEqualRule<R>(value));
        }
        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, R value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotEqualRule<R>(value, comparer));
        }
        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value)
        {
            return mpr.MustPassRule(new NotEqualRule<T, R>(GetExpression(mpr), value));
        }
        public static M MustNotEqual<M, T, R>(this IMustPassRule<M, T, R> mpr, Expression<Func<T, R>> value, IEqualityComparer<R> comparer)
        {
            return mpr.MustPassRule(new NotEqualRule<T, R>(GetExpression(mpr), value, comparer));
        }
        #endregion

        #region Regex
        public static M MustMatchRegex<M, T>(this IMustPassRule<M, T, string> mpr, string pattern)
        {
            return mpr.MustPassRule(new RegexRule(new Regex(pattern, RegexOptions.Compiled)));
        }

        public static M MustMatchRegex<M, T>(this IMustPassRule<M, T, string> mpr, string pattern, RegexOptions options)
        {
            return mpr.MustPassRule(new RegexRule(new Regex(pattern, options)));
        }
        #endregion

        #region Of Type

        public static M MustBeOfType<M, T, R>(this IMustPassRule<M, T, R> mpr, Type type)
        {
            return mpr.MustPassRule(new OfTypeRule<R>(type));
        }
        #endregion


        public static M MustPassGenericRule<M, T, R>(this IMustPassRule<M, T, R> mpr, Func<R, bool> rule)
        {
            return mpr.MustPassRule(new GenericRule<R>(rule));
        }

        public static M CallValidate<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where M : ISetupClass
        {
            return CallValidate(mpr, null);
        }

        public static M CallValidate<M, T, R>(this IMustPassRule<M, T, R> mpr, IEngine usingEngine)
            where M : ISetupClass
        {
            var tmpScope = mpr.FluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.FluentScopeAction,  new FluentScopeAction(BuilderHelper.RegisterComposition));
            tmpScope.Delete(RegistryKeys.Rule);
            if (usingEngine != null)
                tmpScope.Set(RegistryKeys.UsingEngine, usingEngine);
            else
                tmpScope.Delete(RegistryKeys.UsingEngine);

            return (M)mpr.GetSelf(tmpScope);
        }

        public static M CallValidateForEachElement<M, T, R>(this IMustPassRule<M, T, R> mpr)
            where R : IEnumerable
            where M : ISetupClass
        {
            return CallValidateForEachElement(mpr, null);
        }

        public static M CallValidateForEachElement<M, T, R>(this IMustPassRule<M, T, R> mpr, IEngine usingEngine)
            where R : IEnumerable
            where M : ISetupClass
        {
            //TODO: Create a test for testing what happens when you have nodes after the CallValidateForEachElement...
            var tmpScope = mpr.FluentScope.CreateChild();
            tmpScope.Set(RegistryKeys.FluentScopeAction, new FluentScopeAction(BuilderHelper.RegisterEnumerableComposition));
            tmpScope.Delete(RegistryKeys.Rule);
            if (usingEngine != null) 
                tmpScope.Set(RegistryKeys.UsingEngine, usingEngine);
            else
                tmpScope.Delete(RegistryKeys.UsingEngine);

            return (M)mpr.GetSelf(tmpScope);
        }
        private static Expression<Func<T, R>> GetExpression<M, T, R>(IMustPassRule<M, T, R> mpr)
        {
            return (Expression<Func<T, R>>)mpr.FluentScope.Get(RegistryKeys.SetupExpression);
        }
    }
}
