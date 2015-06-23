using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Rules;
using System.Linq.Expressions;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class MessageTests
    {
        private class MyMessageTestClass
        {
            private int _a;
            private int _b;

            /// <summary>
            /// Gets A
            /// </summary>
            public int A
            {
                get { return _a; }
            }

            /// <summary>
            /// Gets B
            /// </summary>
            public int B
            {
                get { return _b; }
            }

            public MyMessageTestClass(int a, int b)
            {
                _a = a;
                _b = b;
            }

        }

        [TestMethod]
        public void TestMessageForSpecificRule()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<MyMessageTestClass>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("Must be Less Than or equal to {0}");

            var engine = builder.Build();
            var report = new TestingValidationReport(engine);
            var obj = new MyMessageTestClass(2, 2);
            report.Validate(obj);
            report.AssertError(obj, o => o.A, RuleKinds.LessThanOrEqualToRule, 1);
        }

        [TestMethod]
        public void TestMessageForProperty()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<MyMessageTestClass>()
                .WithMessage("Object is not valid")
                .Setup(m => m.A)
                    .WithMessage("Property A is not valid.")
                    .MustBeLessThanOrEqualTo(10)
                    .MustNotEqual(5)
                    .MustNotEqual(0)
                    .WithMessage("Must not equal zero")
                .Setup(m => m.B)
                    .MustEqual(0);


            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new MyMessageTestClass(11, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual("Property A is not valid.", report.GetErrorMessage(obj, o => o.A));

            report = new ValidationReport(engine);
            obj = new MyMessageTestClass(5, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual("Property A is not valid.", report.GetErrorMessage(obj, o => o.A));

            report = new ValidationReport(engine);
            obj = new MyMessageTestClass(0, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual("Must not equal zero", report.GetErrorMessage(obj, o => o.A));

            report = new ValidationReport(engine);
            obj = new MyMessageTestClass(9, 1);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual("Object is not valid", report.GetErrorMessage(obj, o => o.B));
        }

        [TestMethod]
        public void TestMessagesCount()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<MyMessageTestClass>()
                .WithMessage("Object is not valid")
                .Setup(m => m.A)
                    .WithMessage("Property A is not valid.")
                    .MustBeBetween(1, 10)
                .Setup(m => m.B)
                    .WithMessage("Property B is not valid.")
                    .MustBeBetween(1, 10);

            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new MyMessageTestClass(0, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual(2, report.GetErrorMessages(obj).Length);

            obj = new MyMessageTestClass(0, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            Assert.AreEqual(2, report.GetErrorMessages(obj).Length);

            //There should be a total of 4 error messages...
            Assert.AreEqual(4, report.GetErrorMessages().Length);
        }
    }
}
