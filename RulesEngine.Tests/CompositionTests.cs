using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RulesEngine;
using System.Collections.ObjectModel;

namespace RulesEngine.Tests
{
    [TestClass]
    public class CompositionTests
    {
        class Foo
        {
            public Foo Foo1 { get; set; }
            public Foo Foo2 { get; set; }
            public int Value { get; set; }
            public Foo(int value)
            {
                Value = value;
            }
        }

        [TestMethod]
        public void ShouldValidateComposition_Explicit()
        {
            var engine = new Engine();
            engine.For<Foo>()
                    .Setup(f => f.Value)
                        .MustEqual(1)
                    .EndSetup()
                    .If(f => f.Foo1 != null)
                        .Setup(f => f.Foo1)
                            .CallValidate(engine)
                    .EndIf()
                    .If(f => f.Foo2 != null)
                        .Setup(f => f.Foo2)
                            .CallValidate(engine);

            var foo = new Foo(1) { Foo1 = new Foo(1), Foo2 = new Foo(1) };
            Assert.IsTrue(engine.Validate(foo));
            foo.Foo2.Value = 3;
            Assert.IsFalse(engine.Validate(foo));
        }

        [TestMethod]
        public void ShouldValidateComposition_ImplicitEngine()
        {
            var engine = new Engine();
            engine.For<Foo>()
                    .Setup(f => f.Value)
                        .MustEqual(1)
                    .EndSetup()
                    .If(f => f.Foo1 != null)
                        .Setup(f => f.Foo1)
                            .CallValidate()
                    .EndIf()
                    .If(f => f.Foo2 != null)
                        .Setup(f => f.Foo2)
                            .CallValidate();

            var foo = new Foo(1) { Foo1 = new Foo(1), Foo2 = new Foo(1) };
            Assert.IsTrue(engine.Validate(foo));
            foo.Foo2.Value = 3;
            Assert.IsFalse(engine.Validate(foo));
        }

        [TestMethod]
        public void ShouldValidateComposition_MultiNested_ImplicitEngine()
        {
            var engine = new Engine();
            engine.For<Foo>()
                    .Setup(f => f.Value)
                        .MustEqual(1)
                    .EndSetup()
                    .If(f => f.Foo1 != null)
                        .If(f => f.Foo1 != null)
                            .If(f => f.Foo1 != null)
                                .Setup(f => f.Foo1)
                                    .CallValidate();
            engine.For<Foo>()
                    .If(f => f.Foo2 != null)
                        .If(f => f.Foo2 != null)
                            .If(f => f.Foo2 != null)
                                .Setup(f => f.Foo2)
                                    .CallValidate();

            var foo = new Foo(1) { Foo1 = new Foo(1), Foo2 = new Foo(1) };
            Assert.IsTrue(engine.Validate(foo));
            foo.Foo2.Value = 3;
            Assert.IsFalse(engine.Validate(foo));
        }


    }
}
