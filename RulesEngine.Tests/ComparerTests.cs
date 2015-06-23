using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RulesEngine.Tests
{
    [TestClass]
    public class ComparerTests
    {
        [TestMethod]
        public void ShouldDifferentiateLambdas()
        {
            //Although the 2 expressions are very similar, the comparer should not be fooled.
            var exp1 = ExpressionHelper.New<Foo, int>(a => a.Value);
            var exp2 = ExpressionHelper.New<Foo2, int>(a => a.Value);

            AssertNotEqual(exp1, exp2, CreateComparer());
        }

        private IEqualityComparer<EquatableExpression> CreateComparer()
        {
            return EquatableExpression.DefaultComparer;
        }



        private void AssertEqual(EquatableExpression exp1, EquatableExpression exp2, IEqualityComparer<EquatableExpression> comparer)
        {
            Assert.IsTrue(comparer.Equals(exp1, exp2), "Expressions are not equal");
            Assert.AreEqual(comparer.GetHashCode(exp1), comparer.GetHashCode(exp2), "Hashcodes do not match");
        }
        private void AssertNotEqual(EquatableExpression exp1, EquatableExpression exp2, IEqualityComparer<EquatableExpression> comparer)
        {
            Assert.IsFalse(comparer.Equals(exp1, exp2), "Expressions are equal");
            Assert.AreNotEqual(comparer.GetHashCode(exp1), comparer.GetHashCode(exp2), "Hashcodes match");
        }

        private void AssertEqual<T, T1>(Expression<Func<T, T1>> exp1, Expression<Func<T, T1>> exp2, IEqualityComparer<EquatableExpression> comparer)
        {
            AssertEqual(new EquatableExpression(exp1), new EquatableExpression(exp2), comparer);
        }
        private void AssertNotEqual<T, T1>(Expression<Func<T, T1>> exp1, Expression<Func<T, T1>> exp2, IEqualityComparer<EquatableExpression> comparer)
        {
            AssertNotEqual(new EquatableExpression(exp1), new EquatableExpression(exp2), comparer);
        }



    }
}
