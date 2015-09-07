using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RulesEngine.Rules;

namespace RulesEngine.Tests
{
    [TestClass]
    public class RulesTests
    {
        private class MyNullableContainer<T>
            where T : struct
        {
            private Nullable<T> _value;

            /// <summary>
            /// Gets or Sets Value
            /// </summary>
            public T? Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public MyNullableContainer(T? value)
            {
                _value = value;
            }
        }

        private class MyDomainObject<T>
        {
            private T _value1;
            private T _value2;
            private T _value3;

            /// <summary>
            /// Gets or Sets Value
            /// </summary>
            public T Value1
            {
                get { return _value1; }
                set { _value1 = value; }
            }

            /// <summary>
            /// Gets or Sets Value2
            /// </summary>
            public T Value2
            {
                get { return _value2; }
                set { _value2 = value; }
            }

            /// <summary>
            /// Gets or Sets Value3
            /// </summary>
            public T Value3
            {
                get { return _value3; }
                set { _value3 = value; }
            }

            public MyDomainObject(T value1)
            {
                _value1 = value1;
                _value2 = value1;
            }

            public MyDomainObject(T value1, T value2)
            {
                _value1 = value1;
                _value2 = value2;
            }

            public MyDomainObject(T value1, T value2, T value3)
            {
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
            }
        }

        [TestMethod]
        public void TestLessThanRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeLessThan(2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1)));
        }

        [TestMethod]
        public void TestLessThanRuleCrossField()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeLessThan(a => a.Value2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 2)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(4, 3);
            v.Validate(o);
            v.AssertError(o, p1 => p1.Value1, RuleKinds.LessThanRule, 3);
        }

        [TestMethod]
        public void TestLessThanOrEqualToRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeLessThanOrEqualTo(2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1)));
        }

        [TestMethod]
        public void TestLessThanOrEqualToRuleCrossField()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeLessThanOrEqualTo(a => a.Value2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 4)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 2)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(4, 3);
            v.Validate(o);
            v.AssertError(o, p1 => p1.Value1, RuleKinds.LessThanOrEqualToRule, 3);
        }

        [TestMethod]
        public void TestGreaterThanRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeGreaterThan(2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(1)));
        }

        [TestMethod]
        public void TestGreaterThanRuleCrossField()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeGreaterThan(a => a.Value2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 3)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 2)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(3, 4);
            v.Validate(o);
            v.AssertError(o, p1 => p1.Value1, RuleKinds.GreaterThanRule, 4);
        }

        [TestMethod]
        public void TestGreaterThanOrEqualToRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeGreaterThanOrEqualTo(2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(1)));
        }

        [TestMethod]
        public void TestGreaterThanOrEqualToRuleCrossField()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeGreaterThanOrEqualTo(a => a.Value2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 4)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 3)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 2)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(3, 4);
            v.Validate(o);

            v.AssertError(o, p1 => p1.Value1, RuleKinds.GreaterThanOrEqualToRule, 4);
        }

        [TestMethod]
        public void TestBetweenRuleCrossFieldLessAndGreater()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(a => a.Value2, a => a.Value3);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 2, 5)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3, 3, 3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 4, 5)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3, 1, 2)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(4, 1, 3);
            v.Validate(o);
            v.AssertError(o, p1 => p1.Value1, RuleKinds.BetweenRule, 1, 3, 4, BetweenRuleBoundsOption.BothInclusive);
        }

        [TestMethod]
        public void TestBetweenRuleCrossFieldLess()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(6, a => a.Value2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(7, 10)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6, 6)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(5, 10)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(8, 7)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(4, 3);
            v.Validate(o);

            v.AssertError(o, p1 => p1.Value1, RuleKinds.BetweenRule, 6, 3, 4, BetweenRuleBoundsOption.BothInclusive);
        }

        [TestMethod]
        public void TestBetweenRuleCrossFieldGreater()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(a => a.Value2, 10);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(7, 5)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6, 6)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(5, 6)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(11, 15)));

            TestingValidationReport v = new TestingValidationReport(engine);
            MyDomainObject<int> o = new MyDomainObject<int>(3, 4);
            engine.Validate(o, v, ValidationReportDepth.FieldShortCircuit);

            v.AssertError(o, p1 => p1.Value1, RuleKinds.BetweenRule, 4, 10, 3, BetweenRuleBoundsOption.BothInclusive);
        }

        [TestMethod]
        public void TestBetweenRuleInclusive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(5, 10);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(10)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(11)));
        }

        [TestMethod]
        public void TestBetweenRuleLowerInclusiveUpperExclusive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(5, 10, Rules.BetweenRuleBoundsOption.LowerInclusiveUpperExclusive);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(10)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(11)));
        }

        [TestMethod]
        public void TestBetweenRuleLowerExclusiveUpperInclusive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(5, 10, Rules.BetweenRuleBoundsOption.LowerExclusiveUpperInclusive);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(10)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(11)));
        }

        [TestMethod]
        public void TestBetweenRuleExclusive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeBetween(5, 10, Rules.BetweenRuleBoundsOption.BothExclusive);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(7)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(6)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(9)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(10)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(11)));
        }

        [TestMethod]
        public void TestEqualRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustEqual(2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(1)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(2)));
        }

        [TestMethod]
        public void TestEqualRuleCrossField1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustEqual(m => m.Value2);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(1, 2)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(4, 4)));
        }

        [TestMethod]
        public void TestNotEqualRule1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustNotEqual(2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(3)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(2)));
        }

        [TestMethod]
        public void TestNotEqualRuleCrossField1()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustNotEqual(m => m.Value2);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1, 2)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4, 4)));
        }


        [TestMethod]
        public void TestMustBeOneOfRule()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustBeOneOf(2, 5, 8, 9);

            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(8)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(9)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(1)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(4)));
        }

        [TestMethod]
        public void TestMustBeOneOfRule_CaseInsensitive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<string>>()
                .Setup(m => m.Value1)
                    .MustBeOneOf(StringComparer.OrdinalIgnoreCase, "BOB", "Paul", "JanE", "Sally");

            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("bob")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("PAUL")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("Sally")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("Jane")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("Ringo")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("Bono")));
        }

        [TestMethod]
        public void TestMustNotBeOneOfRule()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<int>>()
                .Setup(m => m.Value1)
                    .MustNotBeOneOf(2, 5, 8, 9);

            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(2)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(5)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(8)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<int>(9)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(4)));
        }

        [TestMethod]
        public void TestMustNotBeOneOfRule_CaseInsensitive()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<string>>()
                .Setup(m => m.Value1)
                    .MustNotBeOneOf(StringComparer.OrdinalIgnoreCase, "BOB", "Paul", "JanE", "Sally");

            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("bob")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("PAUL")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("Sally")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("Jane")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("Ringo")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("Bono")));
        }

        [TestMethod]
        public void TestMustPassRegex()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<string>>()
                .Setup(m => m.Value1)
                    .MustMatchRegex("^A+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("AAAAAAAA")));
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>("aaaaaa")));
            //NOTE: If the string is null, then Regex should still validate.
            Assert.IsTrue(engine.Validate(new MyDomainObject<string>(null)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<string>("BBBB")));
        }

        [TestMethod]
        public void TestMustBeOfType()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<object>>()
                .Setup(m => m.Value1)
                    .MustBeOfType(typeof(ApplicationException));

            Assert.IsTrue(engine.Validate(new MyDomainObject<object>(new ApplicationException())));
            Assert.IsTrue(engine.Validate(new MyDomainObject<object>(new System.Reflection.TargetException())));
            Assert.IsFalse(engine.Validate(new MyDomainObject<object>(new Exception())));
            Assert.IsFalse(engine.Validate(new MyDomainObject<object>("123")));
            Assert.IsFalse(engine.Validate(new MyDomainObject<object>(null)));
        }

        [TestMethod]
        public void TestNullableNull()
        {
            Engine engine = new Engine();
            engine.For<MyNullableContainer<int>>()
                .Setup(m => m.Value)
                    .MustBeNull();

            Assert.IsTrue(engine.Validate(new MyNullableContainer<int>(null)));
            Assert.IsFalse(engine.Validate(new MyNullableContainer<int>(1)));
        }

        [TestMethod]
        public void TestNullableNotNull()
        {
            Engine engine = new Engine();
            engine.For<MyNullableContainer<int>>()
                .Setup(m => m.Value)
                    .MustNotBeNull();

            Assert.IsFalse(engine.Validate(new MyNullableContainer<int>(null)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<int>(1)));
        }

        [TestMethod]
        public void TestReferenceNull()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<object>>()
                .Setup(m => m.Value1)
                    .MustBeNull();

            Assert.IsTrue(engine.Validate(new MyDomainObject<object>(null)));
            Assert.IsFalse(engine.Validate(new MyDomainObject<object>(new object())));
        }

        [TestMethod]
        public void TestReferenceNotNull()
        {
            Engine engine = new Engine();
            engine.For<MyDomainObject<object>>()
                .Setup(m => m.Value1)
                    .MustNotBeNull();

            Assert.IsFalse(engine.Validate(new MyDomainObject<object>(null)));
            Assert.IsTrue(engine.Validate(new MyDomainObject<object>(new object())));
        }

    }
}
