using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class ValidationReportTests
    {
        private class ClassA
        {
            public int A { get; set; }
        }
        private class ClassB : ClassA
        {
            public int B { get; set; }
        }

        [TestMethod]
        public void ShouldHaveErrorA()
        {
            var r = CreateReport();
            var o = new ClassA();
            //Simulate an error occuring on ClassA
            r.AddError(new ValidationError(CreateRule(), EquatableExpression.Create<ClassA, int>(a => a.A), new object[0], o, o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            
            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            //Eventhough, o is not ClassB, the expression b => b.A still applies to a ClassA. And therefore the following statement must be true.
            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassB, int>(b => b.A)));
        }

        [TestMethod]
        public void ShouldMatchAnyExpression()
        {
            var r = CreateReport();
            var o = new ClassA();
            //Simulate an error occuring on ClassA
            r.AddError(new ValidationError(CreateRule(), EquatableExpression.Create<ClassA, int>(a => a.A), new object[0], o, o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            //Passing in null should match any errors on any expressions...
            Assert.IsTrue(r.HasError(o, null));
        }


        [TestMethod]
        public void ShouldHaveErrorB()
        {
            var r = CreateReport();
            var o = new ClassB();
            //Simulate an error occuring on ClassB for a rule defined on ClassA
            r.AddError(new ValidationError(CreateRule(), EquatableExpression.Create<ClassA, int>(a => a.A), new object[0], o, o, EquatableExpression.Create<ClassA, int>(a => a.A)));

            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            //Eventhough, o is not ClassB, the expression b => b.A still applies to a ClassA. And therefore the following statement must be true.
            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassB, int>(b => b.A)));
        }

        [TestMethod]
        public void ShouldHaveErrorA1()
        {
            var r = CreateReport();
            var o = new ClassA();
            //Simulate an error occuring on ClassB.
            r.AddError(new ValidationError(CreateRule(), EquatableExpression.Create<ClassB, int>(a => a.A), new object[0], o, o, EquatableExpression.Create<ClassB, int>(a => a.A)));

            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            //Eventhough, o is not ClassB, the expression b => b.A still applies to a ClassA. And therefore the following statement must be true.
            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassB, int>(b => b.A)));
        }

        [TestMethod]
        public void ShouldHaveErrorB1()
        {
            var r = CreateReport();
            var o = new ClassB();
            //Simulate an error occuring on ClassB.
            r.AddError(new ValidationError(CreateRule(), EquatableExpression.Create<ClassB, int>(a => a.A), new object[0], o, o, EquatableExpression.Create<ClassB, int>(a => a.A)));

            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassA, int>(a => a.A)));
            //Eventhough, o is not ClassB, the expression b => b.A still applies to a ClassA. And therefore the following statement must be true.
            Assert.IsTrue(r.HasError(o, EquatableExpression.Create<ClassB, int>(b => b.A)));
        }

        [TestMethod]
        public void ShouldValidate()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassA>().Setup(a => a.A).MustBeGreaterThan(0);
            var engine = builder.Build();
            var report = new ValidationReport(engine);
            Assert.IsFalse(report.Validate(new ClassA()));
        }

        
        private IValidationReport CreateReport()
        {
            return new ValidationReport();
        }
        private IRule CreateRule()
        {
            return new Rules.NotNullOrEmpty();
        }
    }
}
