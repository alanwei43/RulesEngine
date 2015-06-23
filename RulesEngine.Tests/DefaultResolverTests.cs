using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Rules;
using System.Linq.Expressions;

namespace RulesEngine.Tests
{
    [TestClass]
    public class DefaultResolverTests
    {
        private class ClassA
        {
            public int A { get; set; }
        }
        private class ClassB : ClassA
        {
            public int B { get; set; }
        }
        private class ClassC : ClassB
        {
            public int C { get; set; }
        }

        [TestMethod]
        public void TestSimple()
        {
            var resolver = new DefaultErrorResolver();
            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A");
            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassA, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
        }

        [TestMethod]
        public void TestInheritance_AllOverridden()
        {
            var resolver = new DefaultErrorResolver();
            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A");
            SetErrorMessage<ClassB, int>(resolver, b => b.A, null, "ClassB validation for property A");
            SetErrorMessage<ClassC, int>(resolver, c => c.A, null, "ClassC validation for property A");

            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassA, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassB validation for property A", GetErrorMessage<ClassB, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassC validation for property A", GetErrorMessage<ClassC, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
        }

        [TestMethod]
        public void TestSimpleInheritance_ShouldGetBaseMessage1()
        {
            var resolver = new DefaultErrorResolver();
            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A");

            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassA, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassB, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassC, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
        }

        [TestMethod]
        public void TestSimpleInheritance_ShouldGetBaseMessage2()
        {
            var resolver = new DefaultErrorResolver();
            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A");
            SetErrorMessage<ClassB, int>(resolver, b => b.A, null, "ClassB validation for property A");

            Assert.AreEqual("ClassA validation for property A", GetErrorMessage<ClassA, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassB validation for property A", GetErrorMessage<ClassB, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassB validation for property A", GetErrorMessage<ClassC, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
        }

        [TestMethod]
        public void TestByExpression()
        {
            var resolver = new DefaultErrorResolver();
            SetErrorMessage<ClassB, int>(resolver, b => b.A, null, "ClassB validation for property A");
            SetErrorMessage<ClassB, int>(resolver, b => b.B, null, "ClassB validation for property B");

            Assert.AreEqual("ClassB validation for property A", GetErrorMessage<ClassB, int>(resolver, o => o.A, new Rules.NotNullOrEmpty()));
            Assert.AreEqual("ClassB validation for property B", GetErrorMessage<ClassB, int>(resolver, o => o.B, new Rules.NotNullOrEmpty()));
        }

        [TestMethod]
        public void TestByRule()
        {
            var resolver = new DefaultErrorResolver();
            var rule1 = new NotNullOrEmpty();
            var rule2 = new RegexRule(new System.Text.RegularExpressions.Regex("a"));
            var rule3 = new Rules.GreaterThanRule<int>(10, true);

            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A - Default");
            SetErrorMessage<ClassA, int>(resolver, a => a.A, rule1, "ClassA validation for property A - Rule1");
            SetErrorMessage<ClassA, int>(resolver, a => a.A, rule2, "ClassA validation for property A - Rule2");

            Assert.AreEqual("ClassA validation for property A - Rule1", GetErrorMessage<ClassA, int>(resolver, o => o.A, rule1));
            Assert.AreEqual("ClassA validation for property A - Rule2", GetErrorMessage<ClassA, int>(resolver, o => o.A, rule2));
            Assert.AreEqual("ClassA validation for property A - Default", GetErrorMessage<ClassA, int>(resolver, o => o.A, rule3));
        }

        [TestMethod]
        public void TestByType()
        {
            var resolver = new DefaultErrorResolver();
            var rule1 = new NotNullOrEmpty();
            var rule2 = new NotNullOrEmpty();

            SetErrorMessage<ClassB, int>(resolver, b => b.A, rule1, "ClassB validation for property A - Rule1");
            SetErrorMessage<ClassB, int>(resolver, b => b.A, null, "ClassB validation for property A - Default");
            SetErrorMessage<ClassB, ClassB>(resolver, b => b, null, "ClassB validation - Default");

            Assert.AreEqual("ClassB validation for property A - Rule1", GetErrorMessage<ClassB, int>(resolver, o => o.A, rule1));
            Assert.AreEqual("ClassB validation for property A - Default", GetErrorMessage<ClassB, int>(resolver, o => o.A, rule2));
            Assert.AreEqual("ClassB validation - Default", GetErrorMessage<ClassB, int>(resolver, o => o.B, new NotNullOrEmpty()));
        }


        [TestMethod]
        public void TestTestByType_ShouldGetOverridenDefault()
        {
            var resolver = new DefaultErrorResolver();
            var rule1 = new NotNullOrEmpty();
            var rule2 = new NotNullOrEmpty();

            SetErrorMessage<ClassA, int>(resolver, a => a.A, rule1, "ClassA validation for property A - Rule1");
            SetErrorMessage<ClassA, int>(resolver, a => a.A, null, "ClassA validation for property A - Default");
            SetErrorMessage<ClassA, ClassA>(resolver, a => a, null, "ClassA validation - Default");
            SetErrorMessage<ClassB, ClassB>(resolver, b => b, null, "ClassB validation - Default");

            //Should get message from default ClassB instead of getting a => a.A from ClassA message!
            Assert.AreEqual("ClassB validation - Default", GetErrorMessage<ClassB, int>(resolver, o => o.A, rule1));
        }

        private void SetErrorMessage<T, R>(IErrorResolver resolver, Expression<Func<T, R>> expression, IRule rule, string message)
        {
            resolver.AddEntry(new MessageEntry(typeof(T), new EquatableExpression(expression), rule, () => message));
        }

        private string GetErrorMessage<T, R>(DefaultErrorResolver resolver, Expression<Func<T, R>> expression, IRule rule)
            where T : new()
        {
            return resolver.GetErrorMessage(typeof(T), new EquatableExpression(expression), rule, new object[0]);
        }
    }
}
