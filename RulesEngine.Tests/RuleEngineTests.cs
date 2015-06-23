using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine.Fluent;

namespace RulesEngine.Tests
{
    [TestClass]
    public class RuleEngineTests
    {
        class Foo1
        {
            public int Value { get; set; }
            public Foo1(int value)
            {
                this.Value = value;
            }
        }
        class Foo2
        {
            public int Value { get; set; }
            public Foo2(int value)
            {
                this.Value = value;
            }
        }
        class Foo3
        {
            public int Value { get; set; }
            public Foo3(int value)
            {
                this.Value = value;
            }
        }

        [TestMethod]
        public void ShouldCopyAllRules()
        {
            var baseBuilder = new Fluent.FluentBuilder();
            baseBuilder.For<Foo1>()
                        .Setup(f => f.Value)
                            .MustBeGreaterThan(0);
            baseBuilder.For<Foo2>()
                        .Setup(f => f.Value)
                            .MustEqual(6);
            
            var baseEngine = baseBuilder.Build();
            Assert.IsFalse(baseEngine.Validate(new Foo1(-1)));
            Assert.IsFalse(baseEngine.Validate(new Foo2(1)));

            var builder = new Fluent.FluentBuilder();
            builder.For<Foo1>()
                    .Setup(f => f.Value)
                        .MustBeLessThan(10);

            var engine1 = builder.Build(baseBuilder);

            //Engine1 now has rules of baseEngine in addition to its own.
            Assert.IsFalse(engine1.Validate(new Foo1(-1)));
            Assert.IsFalse(engine1.Validate(new Foo1(10)));
            Assert.IsTrue(engine1.Validate(new Foo1(9)));
            Assert.IsFalse(engine1.Validate(new Foo2(2)));
            Assert.IsTrue(engine1.Validate(new Foo2(6)));
        }

        [TestMethod]
        public void ShouldCopySpecificRules()
        {
            var baseBuilder = new Fluent.FluentBuilder();
            baseBuilder.For<Foo1>()
                        .Setup(f => f.Value)
                            .MustEqual(1);
            baseBuilder.For<Foo2>()
                        .Setup(f => f.Value)
                            .MustEqual(2);
            baseBuilder.For<Foo3>()
                        .Setup(f => f.Value)
                            .MustEqual(3);

            var builder = new Fluent.FluentBuilder();
            var engine1 = builder.Build(baseBuilder, typeof(Foo1), typeof(Foo3));

            //Engine1 now has rules of baseEngine (only Foo1 and Foo3).
            Assert.IsFalse(engine1.Validate(new Foo1(0)));
            Assert.IsTrue(engine1.Validate(new Foo1(1)));

            Assert.IsTrue(engine1.Validate(new Foo3(3)));
            Assert.IsFalse(engine1.Validate(new Foo3(0)));

            Assert.IsTrue(engine1.Validate(new Foo2(2)));
            Assert.IsTrue(engine1.Validate(new Foo2(0)));
            Assert.IsTrue(engine1.Validate(new Foo2(1)));
        }

    }
}
