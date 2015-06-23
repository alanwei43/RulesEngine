using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    public class ConditionalData
    {
        int _a;
        int _b;
        int _c;

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

        /// <summary>
        /// Gets C
        /// </summary>
        public int C
        {
            get { return _c; }
        }

        public ConditionalData(int a, int b, int c)
        {
            _a = a;
            _b = b;
            _c = c;
        }
    }

    [TestClass]
    public class ConditionalTests
    {
        //TODO: Test with 'if' and 'Composition'. Check, there may already be some.

        [TestMethod]
        public void ShouldWorkWithConditions()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ConditionalData>()
                    .If(m => m.A > 10)
                        .Setup(m => m.B)
                            .MustBeGreaterThan(m => m.C)
                    .Else()
                        .Setup(m => m.B)
                            .MustBeLessThan(m => m.C)
                    .EndIf();

            var data1 = new ConditionalData(11, 1, 2);
            var data2 = new ConditionalData(11, 2, 1);
            var data3 = new ConditionalData(0, 1, 2);
            var data4 = new ConditionalData(0, 2, 1);

            var engine = builder.Build();
            Assert.IsFalse(engine.Validate(data1));
            Assert.IsTrue(engine.Validate(data2));
            Assert.IsTrue(engine.Validate(data3));
            Assert.IsFalse(engine.Validate(data4));
        }

        [TestMethod]
        public void ShouldWorkWithConditions2()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ConditionalData>()
                    .If(m => m.A > 10)
                        .Setup(m => m.B)
                            .MustBeGreaterThan(5)
                    .Else()
                        .Setup(m => m.B)
                            .MustBeLessThan(5)
                    .EndIf();

            var data1 = new ConditionalData(11, 0, 0);
            var data2 = new ConditionalData(11, 6, 0);
            var data3 = new ConditionalData(0, 0, 0);
            var data4 = new ConditionalData(0, 6, 0);

            var engine = builder.Build();
            Assert.IsFalse(engine.Validate(data1));
            Assert.IsTrue(engine.Validate(data2));
            Assert.IsTrue(engine.Validate(data3));
            Assert.IsFalse(engine.Validate(data4));
        }

        
        [TestMethod]
        public void ShouldWorkWithConditions3()
        {
            var builder = new Fluent.FluentBuilder();
            builder.For<ConditionalData>()
                    .If(m => m.A > 5)
                        .If(m => m.A > 10)
                            .Setup(m => m.B)
                                .MustBeGreaterThan(5)
                        .Else()
                            .Setup(m => m.B)
                                .MustBeLessThan(5)
                        .EndIf()
                    .EndIf();


            var data1 = new ConditionalData(11, 0, 0);
            var data2 = new ConditionalData(11, 6, 0);
            var data3 = new ConditionalData(6, 0, 0);
            var data4 = new ConditionalData(6, 6, 0);

            var engine = builder.Build();
            Assert.IsFalse(engine.Validate(data1));
            Assert.IsTrue(engine.Validate(data2));
            Assert.IsTrue(engine.Validate(data3));
            Assert.IsFalse(engine.Validate(data4));
        }
    }
}
