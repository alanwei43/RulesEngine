using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Linq;

namespace RulesEngine.Tests
{
    [TestClass]
    public class DefaultCulpritResolverTests
    {
        private class Dummy
        {
            public Dummy2 Dummy2Field;
            public Dummy2 Dummy2 { get; set; }
            public Dummy2[] Dummy2Array { get; set; }
        }
        private class Dummy2
        {
            public int Value2 { get; set; }
            public int GetCalculatedValue2()
            {
                return Value2 * 3;
            }
        }


        [TestMethod]
        public void ShouldResolve()
        {
            Expression<Func<Dummy, int>> expression = f => f.Dummy2.Value2;
            var resolver = new DefaultCulpritResolver(expression);
            var dummy = new Dummy() { Dummy2 = new Dummy2() };
            var result = resolver.Resolve(dummy).ToArray();
            Assert.AreSame(dummy.Dummy2, result[0].Value);
            Assert.AreEqual(new EquatableExpression(Dummy2Exp(m => m.Value2)), result[0].Expression);

        }

        [TestMethod]
        public void ShouldResolve_Unnormalized()
        {
            var expression = DummyExp(d => d.Dummy2.GetCalculatedValue2());
            var resolver = new DefaultCulpritResolver(expression);
            var dummy = new Dummy() { Dummy2 = new Dummy2() };
            var result = resolver.Resolve(dummy).ToArray();
            //Because expression is not a MemberExpression. The Culprit should stay the root, and the expression should resolve to the same.
            Assert.AreSame(dummy, result[0].Value);
            Assert.AreEqual(new EquatableExpression(DummyExp(d => d.Dummy2.GetCalculatedValue2())), result[0].Expression);
        }
        


        private Expression<Func<Dummy, TProperty>> DummyExp<TProperty>(Expression<Func<Dummy, TProperty>> expression) { return expression; }
        private Expression<Func<Dummy2, TProperty>> Dummy2Exp<TProperty>(Expression<Func<Dummy2, TProperty>> expression) { return expression; }
    }
}
