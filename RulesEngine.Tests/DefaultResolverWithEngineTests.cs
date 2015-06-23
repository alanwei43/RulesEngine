using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class DefaultResolverWithEngineTests
    {
        private class ClassC
        {
            public int C { get; set; }
        }

        private class CompositeClass
        {
            private ClassC _c;
            private int _value;

            /// <summary>
            /// Gets or Sets C
            /// </summary>
            public ClassC C
            {
                get { return _c; }
                set { _c = value; }
            }

            /// <summary>
            /// Gets or Sets Value
            /// </summary>
            public int Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }
        [TestMethod]
        public void ShouldFindErrorMessage()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                    .Setup(c => c.C)
                        .WithMessage("C is not valid")
                        .MustBeBetween(1, 10, Rules.BetweenRuleBoundsOption.BothInclusive);
            builder.For<CompositeClass>()
                    .Setup(c => c.Value)
                        .MustBeGreaterThan(0)
                    .Setup(c => c.C)
                        .CallValidate()
                        .WithMessage("Composed value is not valid");
            
            var engine = builder.Build();

            var o = new CompositeClass() { C = new ClassC() };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("Composed value is not valid", r.GetErrorMessage(o, f => f.C));
            Assert.AreEqual("C is not valid", r.GetErrorMessage(o.C, f => f.C));
        }

        [TestMethod]
        public void ShouldFindErrorMessage2()
        {
            var builder = new Fluent.FluentBuilder();

            builder.For<CompositeClass>()
                    .Setup(c => c.Value)
                        .MustBeGreaterThan(0)
                    .Setup(c => c.C)
                        .MustNotBeNull()
                    .Setup(c => c.C.C)
                        .WithMessage("Composed value is not valid")
                        .MustBeBetween(1, 10, Rules.BetweenRuleBoundsOption.BothInclusive)
                        ;
            var engine = builder.Build();
            var o = new CompositeClass() { C = new ClassC() };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("Composed value is not valid", r.GetErrorMessage(o.C, f => f.C));
            //NOTE: There are no errors on ClassC! Should return null.
            Assert.IsNull(r.GetErrorMessage(o, f => f.C.C));

        }

        [TestMethod]
        public void ShouldFindErrorMessage3()
        {
            var builder = new Fluent.FluentBuilder();

            builder.For<ClassC>().WithMessage("ClassC is not valid")
                .Setup(c => c.C)
                    .MustBeBetween(1, 10);

            builder.For<CompositeClass>()
                    .Setup(c => c.C)
                        .CallValidate();
            
            var engine = builder.Build();
            var o = new CompositeClass() { C = new ClassC() };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("ClassC is not valid", r.GetErrorMessage(o, f => f.C));
        }

        [TestMethod]
        public void ShouldFindErrorMessage4()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                    .Setup(c => c.C)
                        .WithMessage("C is not valid")
                        .MustBeBetween(1, 10, Rules.BetweenRuleBoundsOption.BothInclusive);
            builder.For<CompositeClass>()
                    .Setup(c => c.Value)
                        .MustBeGreaterThan(0)
                    .Setup(c => c.C)
                        .MustNotBeNull()
                        .WithMessage("C Must not be null")
                        .CallValidate()
                        .WithMessage("Composed value is not valid");

            var engine = builder.Build();

            var o = new CompositeClass() { C = new ClassC() };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("Composed value is not valid", r.GetErrorMessage(o, f => f.C));
            Assert.AreEqual("C is not valid", r.GetErrorMessage(o.C, f => f.C));

            var o1 = new CompositeClass() { C = null };
            var r1 = new ValidationReport(engine);
            r1.Validate(o1);
            Assert.AreEqual("C Must not be null", r1.GetErrorMessage(o1, f => f.C));
        }


        [TestMethod]
        public void ShouldFindErrorMessageWithIF()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                .If(c => c.C > 10)
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. >10")
                    .MustBeBetween(10, 15)
                .EndIf();

            var engine = builder.Build();
            var o = new ClassC() { C = 20 };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("C is not valid. >10", r.GetErrorMessage(o, f => f.C));
                
        }

        [TestMethod]
        public void ShouldFindErrorMessageWithElse()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                .If(c => c.C > 10)
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. >10")
                    .MustEqual(-1)
                .Else()
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. <10")
                    .MustEqual(-1)
                .EndIf();

            var engine = builder.Build();
            var o = new ClassC() { C = 5 };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("C is not valid. <10", r.GetErrorMessage(o, f => f.C));

        }

        [TestMethod]
        public void ShouldFindErrorMessageWithNestedIF()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                .If(c => c.C > 10)
                .If(c => c.C > 11)
                .If(c => c.C > 12)
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. >12")
                    .MustBeBetween(10, 15);

            var engine = builder.Build();
            var o = new ClassC() { C = 20 };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("C is not valid. >12", r.GetErrorMessage(o, f => f.C));

        }

        [TestMethod]
        public void ShouldFindErrorMessageWithNestedIF_Else()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ClassC>()
                .If(c => c.C > 1)
                .If(c => c.C > 2)
                .If(c => c.C > 5)
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. >5")
                    .MustEqual(18)
                .Else()
                    .Setup(c => c.C)
                    .WithMessage("C is not valid. <5")
                    .MustEqual(1);

            var engine = builder.Build();
            var o = new ClassC() { C = 4 };
            var r = new ValidationReport(engine);
            r.Validate(o);
            Assert.AreEqual("C is not valid. <5", r.GetErrorMessage(o, f => f.C));

        }

    }
}
