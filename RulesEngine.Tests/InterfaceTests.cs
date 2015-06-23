using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Rules;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class InterfaceTests
    {
        private interface IMyClass
        {
            int A { get; set; }
        }
        private class MyClass : IMyClass
        {
            int _a;

            /// <summary>
            /// Gets or Sets A
            /// </summary>
            public int A
            {
                get { return _a; }
                set { _a = value; }
            }

            public MyClass(int a)
            {
                _a = a;
            }
        }
        private class MyClassExplicit : IMyClass
        {
            private int _a;
            int IMyClass.A
            {
                get
                {
                    return _a;
                }
                set
                {
                    _a = value;
                }
            }

            public MyClassExplicit(int a)
            {
                _a = a;
            }
        }



        [TestMethod]
        public void SimpleInterfaceTest()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<IMyClass>()
                    .Setup(m => m.A)
                        .MustBeGreaterThan(0);
            
            var engine = builder.Build();
            var report = new TestingValidationReport(engine);

            var o1 = new MyClass(0);
            Assert.IsFalse(report.Validate(o1));
            Assert.AreEqual(1, report.Errors.Length);
            report.AssertError<IMyClass, int>(o1, p1 => p1.A, RuleKinds.GreaterThanRule, 0);

            var o2 = new MyClassExplicit(0);
            Assert.IsFalse(report.Validate(o2));
            Assert.AreEqual(1, report.Errors.Length);
            report.AssertError<IMyClass, int>(o2, p1 => p1.A, RuleKinds.GreaterThanRule, 0);
        }
    }

}
