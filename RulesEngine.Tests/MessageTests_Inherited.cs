using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class InheritedMessageTests
    {
        private class ClassA
        {
            private int _a;

            /// <summary>
            /// Gets A
            /// </summary>
            public int A
            {
                get { return _a; }
            }

            public ClassA(int a)
            {
                _a = a;
            }
        }
        private class ClassB : ClassA
        {
            private int _b;

            /// <summary>
            /// Gets B
            /// </summary>
            public int B
            {
                get { return _b; }
            }

            public ClassB(int a, int b)
                :base(a)
            {
                _b = b;
            }
        }
        private class ClassC : ClassB
        {
            private int _c;

            /// <summary>
            /// Gets C
            /// </summary>
            public int C
            {
                get { return _c; }
            }

            public ClassC(int a, int b, int c)
                : base(a, b)
            {
                _c = c;
            }

        }


        [TestMethod]
        public void TestMessageInheritedClass()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            builder.For<ClassB>()
                .Setup(m => m.A)
                    .WithMessage("ClassB validation for A");

            builder.For<ClassC>()
                .Setup(m => m.A)
                    .WithMessage("ClassC validation for A");

            var engine = builder.Build(); 
            var report = new ValidationReport(engine);
            var obj = new ClassC(2,0,0);
            report.Validate(obj);
            Assert.AreEqual("ClassC validation for A", report.GetErrorMessage(obj, o => o.A));
        }

        [TestMethod]
        public void TestMessageInheritedClass2()
        {
            //NOTE: Same as TestMessageInheritedClass, with order of registrations changed.

            var builder = new Fluent.FluentBuilder();

            builder.For<ClassC>()
                .Setup(m => m.A)
                    .WithMessage("ClassC validation for A");

            builder.For<ClassB>()
                .Setup(m => m.A)
                    .WithMessage("ClassB validation for A");

            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new ClassC(2, 0, 0);
            report.Validate(obj);
            Assert.AreEqual("ClassC validation for A", report.GetErrorMessage(obj, o => o.A));
        }

        [TestMethod]
        public void TestMessageInheritedClass3()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            builder.For<ClassB>()
                .Setup(m => m.A)
                    .WithMessage("ClassB validation for A");

            builder.For<ClassC>()
                .Setup(m => m.A)
                    .WithMessage("ClassC validation for A.1")
                    .MustBeLessThan(-1)
                    .WithMessage("ClassC validation for A.2");

            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new ClassC(2, 0, 0);
            report.Validate(obj);
            //m=>m.A will hit A.1 because the rules for base types are executed first. m=>m.A will fail because of rule defined on ClassA!.
            Assert.AreEqual("ClassC validation for A.1", report.GetErrorMessage(obj, o => o.A));

            obj = new ClassC(1, 0, 0);
            report.Validate(obj);
            //m=>m.A will hit A.2 because the rules for base types are fine. m=>m.A will fail because of rule defined on ClassC!.
            Assert.AreEqual("ClassC validation for A.2", report.GetErrorMessage(obj, o => o.A));
        }

        [TestMethod]
        public void TestMessageInheritedClass4()
        {
            //NOTE: This test is the same as No.3, with definition of rules order changed.
            var builder = new Fluent.FluentBuilder();

            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            builder.For<ClassB>()
                .Setup(m => m.A)
                    .WithMessage("ClassB validation for A");

            builder.For<ClassC>()
                .Setup(m => m.A)
                    .WithMessage("ClassC validation for A.1")
                    .MustBeLessThan(-1)
                    .WithMessage("ClassC validation for A.2");


            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new ClassC(2, 0, 0);
            report.Validate(obj);
            //m=>m.A will hit A.1 because the rules for base types are executed first. m=>m.A will fail because of rule defined on ClassA!.
            Assert.AreEqual("ClassC validation for A.1", report.GetErrorMessage(obj, o => o.A));

            report = new ValidationReport(engine);
            obj = new ClassC(1, 0, 0);
            engine.Validate(obj, report, ValidationReportDepth.FieldShortCircuit);
            //m=>m.A will hit A.2 because the rules for base types are fine. m=>m.A will fail because of rule defined on ClassC!.
            Assert.AreEqual("ClassC validation for A.2", report.GetErrorMessage(obj, o => o.A));

        }

        [TestMethod]
        public void TestMessageInheritedClass5()
        {
            var builder = new Fluent.FluentBuilder();

            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            builder.For<ClassB>()
                .Setup(m => m.B)
                    .WithMessage("ClassB validation for B");

            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new ClassB(2, 0);
            report.Validate(obj);
            Assert.AreEqual("ClassA validation for A", report.GetErrorMessage(obj, o => o.A));
            Assert.AreEqual("ClassA validation for A", report.GetErrorMessage((ClassA)obj, o => o.A));
        }

        [TestMethod]
        public void TestMessageInheritedClass6()
        {
            var builder = new Fluent.FluentBuilder();

            builder.For<ClassA>()
                .Setup(m => m.A)
                    .MustBeLessThanOrEqualTo(1)
                    .WithMessage("ClassA validation for A");

            builder.For<ClassB>()
                .Setup(m => m.A)
                    .WithMessage("ClassB validation for A");

            var engine = builder.Build();
            var report = new ValidationReport(engine);
            var obj = new ClassB(2, 0);
            report.Validate(obj);
            Assert.AreEqual("ClassB validation for A", report.GetErrorMessage(obj, o => o.A));
            Assert.AreEqual("ClassB validation for A", report.GetErrorMessage((ClassA)obj, o => o.A));
        }

    
    }
}
